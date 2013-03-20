#region Using

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace Libra.Content.Xnb
{
    public sealed class XnbTypeReaderManager
    {
        // Dictionary だが値がキーと同一であるハッシュセット。
        Dictionary<Type, Type> genericTypeReaderDefinitions;

        Dictionary<Type, XnbTypeReader> typeReadersByTargetType;

        Dictionary<Type, XnbTypeReader> typeReadersByType;

        Dictionary<string, Type> typeReaderTypeByXnaTypeName;

        public XnbTypeReaderManager()
        {
            genericTypeReaderDefinitions = new Dictionary<Type, Type>();
            typeReadersByTargetType = new Dictionary<Type, XnbTypeReader>();
            typeReadersByType = new Dictionary<Type, XnbTypeReader>();
            typeReaderTypeByXnaTypeName = new Dictionary<string, Type>();
        }

        public XnbTypeReader GetTypeReader(Type targetType)
        {
            // 型に対応するインスタンスの検索。
            var typeReader = FindExistingTypeReader(targetType);

            // ジェネリクス型定義からのインスタンスの検索。
            if (typeReader == null && targetType.IsGenericType)
            {
                typeReader = FindGenericTypeReader(targetType);
            }

            // インスタンスが見つからない場合はエラー。
            if (typeReader == null)
                throw new InvalidOperationException("XnbTypeReader not found: " + targetType);

            // インスタンスが見つかった場合は必要に応じて初期化。
            if (!typeReader.Initialized)
                typeReader.Initialize(this);

            return typeReader;
        }

        public XnbTypeReader GetTypeReaderByXnaTypeName(string xnaTypeName)
        {
            if (xnaTypeName == null) throw new ArgumentNullException("xnaTypeName");

            var type = typeReaderTypeByXnaTypeName[xnaTypeName];

            var typeReader = typeReadersByType[type];

            if (!typeReader.Initialized)
                typeReader.Initialize(this);

            return typeReader;
        }

        public void Load(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (!IsXnbTypeReaderType(type)) throw new ArgumentException("Invalid type: " + type, "type");

            if (type.IsGenericTypeDefinition)
            {
                // ListWriter などのジェネリクス型定義はここに入る。
                // 具体的な型引数によるインスタンス化は、
                // 型引数を決定できる状態になるまで保留される。
                genericTypeReaderDefinitions[type] = type;
            }
            else
            {
                // 型が明確な場合は、ここでインスタンス化して登録する。
                var typeReader = Activator.CreateInstance(type) as XnbTypeReader;
                typeReadersByTargetType[typeReader.TargetType] = typeReader;
                typeReadersByType[type] = typeReader;
            }

            var attribute = Attribute.GetCustomAttribute(type, typeof(XnbTypeReaderAttribute)) as XnbTypeReaderAttribute;
            typeReaderTypeByXnaTypeName[attribute.XnaTypeName] = type;
        }

        public void LoadFrom(Assembly assembly)
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
                if (IsXnbTypeReaderType(type))
                    Load(type);
            }
        }

        public void LoadFrom(AppDomain appDomain)
        {
            if (appDomain == null) throw new ArgumentNullException("appDomain");

            foreach (var assembly in appDomain.GetAssemblies())
            {
                LoadFrom(assembly);
            }
        }

        XnbTypeReader FindGenericTypeReader(Type targetType)
        {
            var genericArguments = targetType.GetGenericArguments();

            foreach (var genericWriterTypeDefinition in genericTypeReaderDefinitions.Keys)
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

                var typeReader = Activator.CreateInstance(concreteTypeReaderType) as XnbTypeReader;
                if (typeReader.TargetType.IsAssignableFrom(targetType))
                {
                    typeReadersByTargetType[typeReader.TargetType] = typeReader;
                    typeReadersByType[concreteTypeReaderType] = typeReader;
                    return typeReader;
                }
            }

            return null;
        }

        XnbTypeReader FindExistingTypeReader(Type targetType)
        {
            XnbTypeReader typeReader;
            if (typeReadersByTargetType.TryGetValue(targetType, out typeReader))
            {
                return typeReader;
            }

            foreach (var interfaceType in targetType.GetInterfaces())
            {
                var typeReaderByInterface = FindExistingTypeReader(interfaceType);
                if (typeReaderByInterface != null)
                {
                    return typeReaderByInterface;
                }
            }

            if (targetType.BaseType != null)
            {
                return FindExistingTypeReader(targetType.BaseType);
            }

            return null;
        }

        static bool IsXnbTypeReaderType(Type type)
        {
            return !type.IsAbstract &&
                typeof(XnbTypeReader).IsAssignableFrom(type) &&
                Attribute.IsDefined(type, typeof(XnbTypeReaderAttribute));
        }
    }
}
