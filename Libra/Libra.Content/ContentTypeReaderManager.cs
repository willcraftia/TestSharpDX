#region Using

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace Libra.Content
{
    public sealed class ContentTypeReaderManager
    {
        Dictionary<Type, ContentTypeReader> typeReaderMap;

        Dictionary<Type, Type> genericTypeReaderDefinitionMap;

        public ContentTypeReader this[Type type]
        {
            get
            {
                // 型に対応するインスタンスの検索。
                var typeReader = FindExistingTypeReader(type);

                // ジェネリクス型定義からのインスタンスの検索。
                if (typeReader == null && type.IsGenericType)
                {
                    typeReader = FindGenericTypeReader(type);
                }

                // インスタンスが見つからない場合はエラー。
                if (typeReader == null)
                    throw new InvalidOperationException("ContentTypeReader not found: " + type);

                // インスタンスが見つかった場合は必要に応じて初期化。
                if (!typeReader.Initialized)
                    typeReader.Initialize(this);

                return typeReader;
            }
            set
            {
                typeReaderMap[type] = value;
            }
        }

        public ContentTypeReaderManager()
        {
            typeReaderMap = new Dictionary<Type, ContentTypeReader>();
            genericTypeReaderDefinitionMap = new Dictionary<Type, Type>();
        }

        public void Add(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (type.IsAbstract)
                throw new ArgumentException("Type must be not abstract: " + type, "type");
            if (!IsTypeReaderType(type))
                throw new ArgumentException("Type is not a ContentTypeReader: " + type, "type");
            if (!HasTypeReaderAttribute(type))
                throw new ArgumentException("Type does not have ContentTypeReaderAttribute: " + type, "type");

            if (type.IsGenericTypeDefinition)
            {
                // ListWriter などのジェネリクス型定義はここに入る。
                // 具体的な型引数によるインスタンス化は、
                // 型引数を決定できる状態になるまで保留される。
                genericTypeReaderDefinitionMap[type] = type;
            }
            else
            {
                // 型が明確な場合は、ここでインスタンス化して登録する。
                var typeReader = Activator.CreateInstance(type) as ContentTypeReader;
                typeReaderMap[typeReader.TargetType] = typeReader;
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
                if (type.IsAbstract || !IsTypeReaderType(type) || !HasTypeReaderAttribute(type))
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

        ContentTypeReader FindGenericTypeReader(Type type)
        {
            var genericArguments = type.GetGenericArguments();

            foreach (var genericWriterTypeDefinition in genericTypeReaderDefinitionMap.Keys)
            {
                var targetGenericArguments = genericWriterTypeDefinition.GetGenericArguments();
                if (genericArguments.Length != targetGenericArguments.Length)
                    continue;

                Type concreteTypeReaderType;
                try
                {
                    concreteTypeReaderType = genericWriterTypeDefinition.MakeGenericType(genericArguments);
                }
                catch
                {
                    continue;
                }

                var concreteTypeReader = Activator.CreateInstance(concreteTypeReaderType) as ContentTypeReader;
                if (concreteTypeReader.TargetType.IsAssignableFrom(type))
                {
                    typeReaderMap[concreteTypeReader.TargetType] = concreteTypeReader;
                    return concreteTypeReader;
                }
            }

            return null;
        }

        ContentTypeReader FindExistingTypeReader(Type type)
        {
            ContentTypeReader typeReader;
            if (typeReaderMap.TryGetValue(type, out typeReader))
            {
                return typeReader;
            }

            foreach (var interfaceType in type.GetInterfaces())
            {
                var typeReaderByInterface = FindExistingTypeReader(interfaceType);
                if (typeReaderByInterface != null)
                {
                    return typeReaderByInterface;
                }
            }

            if (type.BaseType != null)
            {
                return FindExistingTypeReader(type.BaseType);
            }

            return null;
        }

        static bool IsTypeReaderType(Type type)
        {
            return typeof(ContentTypeReader).IsAssignableFrom(type);
        }

        static bool HasTypeReaderAttribute(Type type)
        {
            return Attribute.IsDefined(type, typeof(ContentTypeReaderAttribute));
        }
    }
}
