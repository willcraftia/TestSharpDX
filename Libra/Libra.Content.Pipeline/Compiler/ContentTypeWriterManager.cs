#region Using

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace Libra.Content.Pipeline.Compiler
{
    public sealed class ContentTypeWriterManager
    {
        Dictionary<Type, ContentTypeWriter> typeWriters;

        Dictionary<Type, Type> genericTypeWriterDefinitions;

        public ContentTypeWriter this[Type type]
        {
            get
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
                    typeWriter.Initialize(this);

                return typeWriter;
            }
            set
            {
                typeWriters[type] = value;
            }
        }

        public ContentTypeWriterManager()
        {
            typeWriters = new Dictionary<Type, ContentTypeWriter>();
            genericTypeWriterDefinitions = new Dictionary<Type, Type>();
        }

        public void Add(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (type.IsAbstract)
                throw new ArgumentException("Type must be not abstract: " + type, "type");
            if (!IsTypeWriterType(type))
                throw new ArgumentException("Type is not a ContentTypeWriter: " + type, "type");
            if (!HasTypeWriterAttribute(type))
                throw new ArgumentException("Type does not have ContentTypeWriterAttribute: " + type, "type");

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

        public void FindAndAddFrom(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentException("assembly");

            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch
            {
                return;
            }

            foreach (var type in types)
            {
                if (type.IsAbstract || !IsTypeWriterType(type) || !HasTypeWriterAttribute(type))
                    continue;

                Add(type);
            }
        }

        public void FindAndAddFrom(AppDomain appDomain)
        {
            if (appDomain == null) throw new ArgumentNullException("appDomain");

            foreach (var assembly in appDomain.GetAssemblies())
            {
                FindAndAddFrom(assembly);
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

        static bool IsTypeWriterType(Type type)
        {
            return typeof(ContentTypeWriter).IsAssignableFrom(type);
        }

        static bool HasTypeWriterAttribute(Type type)
        {
            return Attribute.IsDefined(type, typeof(ContentTypeWriterAttribute));
        }
    }
}
