#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Libra.Content.Pipeline.Compiler
{
    public sealed class ContentCompiler
    {
        Dictionary<Type, ContentTypeWriter> typeWriters;

        Dictionary<Type, Type> genericTypeWriterDefinitions;

        public ContentCompiler()
        {
            typeWriters = new Dictionary<Type, ContentTypeWriter>();
            genericTypeWriterDefinitions = new Dictionary<Type, Type>();

            CollectTypeWriters();
        }

        public ContentTypeWriter GetTypeWriter(Type type)
        {
            var typeWriter = FindExistingTypeWriter(type);
            if (typeWriter != null)
            {
                if (!typeWriter.Initialized)
                    typeWriter.InternalInitialize(this);

                return typeWriter;
            }

            // ジェネリクス型定義からのインスタンス化を試行。
            if (type.IsGenericType)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();
                var genericTypeWriterDefinition = FindGenericTypeWriterDefinition(genericTypeDefinition);
                if (genericTypeWriterDefinition != null)
                {
                    var genericTypeWriterType = genericTypeWriterDefinition.MakeGenericType(type.GetGenericArguments());
                    typeWriter = Activator.CreateInstance(genericTypeWriterType) as ContentTypeWriter;
                    typeWriters[typeWriter.TargetType] = typeWriter;

                    if (!typeWriter.Initialized)
                        typeWriter.InternalInitialize(this);

                    return typeWriter;
                }
            }

            throw new InvalidOperationException("ContentTypeWriter not found: " + type);
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
