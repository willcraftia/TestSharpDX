#region Using

using System;
using System.Collections.Generic;
using System.IO;
using Libra.Content.Pipeline.Processors;

#endregion

namespace Libra.Content.Pipeline.Compiler
{
    public sealed class ContentCompiler
    {
        ContentCompilerFactory factory;

        Dictionary<Type, ContentTypeWriter> typeWriters;

        Dictionary<Type, Type> genericTypeWriterDefinitions;

        internal ContentCompiler(ContentCompilerFactory factory)
        {
            if (factory == null) throw new ArgumentNullException("factory");

            this.factory = factory;

            typeWriters = new Dictionary<Type, ContentTypeWriter>();
            genericTypeWriterDefinitions = new Dictionary<Type, Type>();

            CollectTypeWriters();
        }

        public ContentTypeWriter GetTypeWriter(Type type)
        {
            // 型に対応するインスタンスの検索。
            var typeWriter = FindExistingTypeWriter(type);

            // ジェネリクス型定義からのインスタンスの検索。
            if (typeWriter == null && type.IsGenericType)
            {
                typeWriter = FindGenericTypeWriter(type);
            }

            // インスタンスが見つからない場合はエラー。
            if (typeWriter == null)
                throw new InvalidOperationException("ContentTypeWriter not found: " + type);

            // インスタンスが見つかった場合は必要に応じて初期化。
            if (!typeWriter.Initialized)
                typeWriter.InternalInitialize(this);

            return typeWriter;
        }

        public string Compile(string sourcePath, IContentProcessor processor, string outputDirectory = null)
        {
            if (string.IsNullOrEmpty(sourcePath)) throw new ArgumentException("sourcePath must be not null/empty.", "sourcePath");
            if (processor == null) throw new ArgumentNullException("processor");

            // ソースのオブジェクト化。
            var source = DeserializeSource(sourcePath);

            // 加工。
            var artifact = processor.Process(source);

            // バイナリ永続化。
            return Write(sourcePath, outputDirectory, artifact);
        }

        public string Compile(
            string contentDescriptionPath,
            string processorName, Dictionary<string, string> processorProperties,
            string outputDirectory = null)
        {
            if (string.IsNullOrEmpty(processorName))
                throw new ArgumentException("processorName must be not null/empty.", "processorName");

            var processor = CreateProcessor(processorName, processorProperties);

            return Compile(contentDescriptionPath, processor, outputDirectory);
        }

        object DeserializeSource(string path)
        {
            var extension = Path.GetExtension(path);
            var serializer = factory.Serializers[extension];

            using (var stream = File.OpenRead(path))
            {
                return serializer.Deserialize(stream);
            }
        }

        IContentProcessor CreateProcessor(string name, Dictionary<string, string> propertyMap)
        {
            var type = factory.ProcessorTypes[name];
            var processor = Activator.CreateInstance(type) as IContentProcessor;

            if (propertyMap != null && propertyMap.Count != 0)
            {
                foreach (var propertyName in propertyMap.Keys)
                {
                    var property = type.GetProperty(propertyName);
                    property.SetValue(processor, propertyMap[propertyName], null);
                }
            }

            return processor;
        }

        string Write(string contentDescriptionPath, string outputDirectory, object content)
        {
            // .ccb (Compiled Content Binary)
            var filename = Path.GetFileNameWithoutExtension(contentDescriptionPath) + ".ccb";

            string outputPath;
            if (string.IsNullOrEmpty(outputDirectory))
            {
                outputPath = filename;
            }
            else
            {
                outputPath = Path.Combine(outputDirectory, filename);
            }

            using (var stream = File.Create(outputPath))
            {
                Write(stream, content);
            }

            return outputPath;
        }

        void Write(Stream stream, object content)
        {
            using (var writer = new ContentWriter(stream, this))
            {
                writer.WriteObject(content);
                writer.Flush();
            }
        }

        ContentTypeWriter FindGenericTypeWriter(Type type)
        {
            var genericArguments = type.GetGenericArguments();

            foreach (var genericWriterTypeDefinition in genericTypeWriterDefinitions.Keys)
            {
                var targetGenericArguments = genericWriterTypeDefinition.GetGenericArguments();
                if (genericArguments.Length != targetGenericArguments.Length)
                    continue;

                Type concreteTypeWriterType;
                try
                {
                    concreteTypeWriterType = genericWriterTypeDefinition.MakeGenericType(genericArguments);
                }
                catch
                {
                    continue;
                }

                var concreteTypeWriter = Activator.CreateInstance(concreteTypeWriterType) as ContentTypeWriter;
                if (concreteTypeWriter.TargetType.IsAssignableFrom(type))
                {
                    typeWriters[concreteTypeWriter.TargetType] = concreteTypeWriter;
                    return concreteTypeWriter;
                }
            }

            return null;
        }

        ContentTypeWriter FindExistingTypeWriter(Type type)
        {
            ContentTypeWriter typeWriter;
            if (typeWriters.TryGetValue(type, out typeWriter))
            {
                return typeWriter;
            }

            foreach (var interfaceType in type.GetInterfaces())
            {
                var typeWriterByInterface = FindExistingTypeWriter(interfaceType);
                if (typeWriterByInterface != null)
                {
                    return typeWriterByInterface;
                }
            }

            if (type.BaseType != null)
            {
                return FindExistingTypeWriter(type.BaseType);
            }

            return null;
        }

        Type FindGenericTypeWriterDefinition(Type type)
        {
            if (genericTypeWriterDefinitions.ContainsKey(type))
                return type;

            foreach (var interfaceType in type.GetInterfaces())
            {
                var genericTypeWriterTypeByInterface = FindGenericTypeWriterDefinition(interfaceType);
                if (genericTypeWriterTypeByInterface != null)
                {
                    return genericTypeWriterTypeByInterface;
                }
            }

            return null;
        }

        void CollectTypeWriters()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch
                {
                    continue;
                }

                var typeWriterType = typeof(ContentTypeWriter);
                var typeWriterAttributeType = typeof(ContentTypeWriterAttribute);

                foreach (var type in types)
                {
                    if (type.IsAbstract)
                        continue;

                    if (!typeWriterType.IsAssignableFrom(type))
                        continue;

                    if (!Attribute.IsDefined(type, typeWriterAttributeType))
                        continue;

                    if (type.IsGenericTypeDefinition)
                    {
                        // ListWriter などのジェネリクス型定義はここに入る。
                        // 具体的な型引数によるインスタンス化は、
                        // 型引数を決定できる状態になるまで保留される。
                        genericTypeWriterDefinitions[type] = type;
                    }
                    else
                    {
                        // 型が明確な場合は、ここでインスタンス化して登録する。
                        var typeWriter = Activator.CreateInstance(type) as ContentTypeWriter;
                        typeWriters[typeWriter.TargetType] = typeWriter;
                    }
                }
            }
        }
    }
}
