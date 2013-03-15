#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Libra.Content.Pipeline.Compiler
{
    public sealed class ContentTypeWriterManager
    {
        Dictionary<Type, ContentTypeWriter> typeWriters;

        Dictionary<Type, Type> genericTypeWriterDefinitions;

        public ContentTypeWriterManager()
        {
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
