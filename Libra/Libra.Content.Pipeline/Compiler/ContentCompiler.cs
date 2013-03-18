#region Using

using System;
using System.Collections.Generic;
using System.IO;
using Libra.Content.Pipeline.Processors;
using Libra.Content.Serialization;

#endregion

namespace Libra.Content.Pipeline.Compiler
{
    public sealed class ContentCompiler
    {
        ContentCompilerFactory factory;

        internal ContentCompiler(ContentCompilerFactory factory)
        {
            if (factory == null) throw new ArgumentNullException("factory");

            this.factory = factory;
        }

        // sourcePath には、factory の SourceRootDirectory からの相対パスを指定。

        public string Compile<TSerializer, TProcessor>(string sourcePath, Dictionary<string, object> processorProperties = null)
            where TSerializer : IContentSerializer, new()
            where TProcessor : IContentProcessor, new()
        {
            var serializer = new TSerializer();

            var processor = new TProcessor();
            PopulateProperties(processor, processorProperties);

            return Compile(sourcePath, serializer, processor);
        }

        public string Compile(string sourcePath, IContentSerializer serializer, IContentProcessor processor)
        {
            if (string.IsNullOrEmpty(sourcePath)) throw new ArgumentException("sourcePath must be not null/empty.", "sourcePath");
            if (serializer == null) throw new ArgumentNullException("serializer");
            if (processor == null) throw new ArgumentNullException("processor");

            // ソースのオブジェクト化。
            var source = DeserializeSource(sourcePath, serializer);

            // 加工。
            var artifact = processor.Process(source);

            // バイナリ永続化。
            return Write(sourcePath, artifact);
        }

        // シリアライザとプロセッサの名前指定は、ビルド ファイル等からの呼び出しを想定。

        public string Compile(
            string sourcePath,
            string serializerName,
            string processorName, Dictionary<string, object> processorProperties = null)
        {
            if (string.IsNullOrEmpty(serializerName))
                throw new ArgumentException("serializerName must be not null/empty.", "serializerName");
            if (string.IsNullOrEmpty(processorName))
                throw new ArgumentException("processorName must be not null/empty.", "processorName");

            var serializer = GetSerializer(serializerName);

            var processor = CreateProcessor(processorName, processorProperties);

            return Compile(sourcePath, serializer, processor);
        }

        object DeserializeSource(string path, IContentSerializer serializer)
        {
            var extension = Path.GetExtension(path);

            var targetPath = (factory.SourceRootDirectory == null) ? path : Path.Combine(factory.SourceRootDirectory, path);

            using (var stream = File.OpenRead(targetPath))
            {
                return serializer.Deserialize(stream);
            }
        }

        IContentSerializer GetSerializer(string name)
        {
            return factory.Serializers[name];
        }

        IContentProcessor CreateProcessor(string name, Dictionary<string, object> propertyMap)
        {
            var type = factory.ProcessorTypes[name];
            var processor = Activator.CreateInstance(type) as IContentProcessor;

            PopulateProperties(processor, propertyMap);

            return processor;
        }

        void PopulateProperties(IContentProcessor processor, Dictionary<string, object> propertyMap)
        {
            if (propertyMap != null && propertyMap.Count != 0)
            {
                var type = processor.GetType();

                foreach (var propertyName in propertyMap.Keys)
                {
                    var property = type.GetProperty(propertyName);
                    property.SetValue(processor, propertyMap[propertyName], null);
                }
            }
        }

        string Write(string sourcePath, object content)
        {
            // .ccb (Compiled Content Binary)
            var filename = Path.GetFileNameWithoutExtension(sourcePath) + ".ccb";

            var sourceDirectoryPath = Path.GetDirectoryName(sourcePath);

            var filePath = Path.Combine(sourceDirectoryPath, filename);

            string outputPath;
            if (factory.OutputRootDirectory == null)
            {
                outputPath = filePath;
            }
            else
            {
                outputPath = Path.Combine(factory.OutputRootDirectory, filePath);
            }

            var outputDirectory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            using (var stream = File.Create(outputPath))
            {
                Write(stream, content);
            }

            return outputPath;
        }

        void Write(Stream stream, object content)
        {
            using (var writer = new ContentWriter(stream, factory.TypeWriters))
            {
                writer.WriteObject(content);
                writer.Flush();
            }
        }
    }
}
