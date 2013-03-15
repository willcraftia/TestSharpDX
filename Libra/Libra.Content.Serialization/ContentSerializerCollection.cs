#region Using

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace Libra.Content.Serialization
{
    // 型やアセンブリからシリアライザを検索して登録する機能は、
    // IContentSerializer かつ ContentSerializerAttribute が設定されていることが条件。
    // インデクサで明示的にインスタンスを設定する場合は、
    // ContentSerializerAttribute を参照せずに拡張子の明示で設定。

    public sealed class ContentSerializerCollection
    {
        // キー: 拡張子 (ドット付き)
        Dictionary<string, IContentSerializer> serializerMap;

        public IContentSerializer this[string extension]
        {
            get { return serializerMap[extension]; }
            set { serializerMap[extension] = value; }
        }

        public ContentSerializerCollection()
        {
            serializerMap = new Dictionary<string, IContentSerializer>();
        }

        public bool Contains(string extension)
        {
            return serializerMap.ContainsKey(extension);
        }

        public void Add(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            if (!IsSerializer(type))
                throw new ArgumentException("Type is not a IContentSerializer: " + type, "type");

            var serializerAttribute = GetSerializerAttribute(type);
            if (serializerAttribute == null)
                throw new ArgumentException("Type does not have ContentSerializerAttribute: " + type, "type");

            var serializer = Activator.CreateInstance(type) as IContentSerializer;

            foreach (var extension in serializerAttribute.Extensions)
            {
                serializerMap[extension] = serializer;
            }
        }

        public void FindAndAddFrom(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

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
                if (!IsSerializer(type))
                    continue;

                if (!HasSerializerAttribute(type))
                    continue;

                Add(type);
            }
        }
        
        public void FindAndAddFrom(AppDomain appDomain)
        {
            foreach (var assembly in appDomain.GetAssemblies())
            {
                FindAndAddFrom(assembly);
            }
        }

        public bool Remove(string extension)
        {
            return serializerMap.Remove(extension);
        }

        public void Clear()
        {
            serializerMap.Clear();
        }

        static bool IsSerializer(Type type)
        {
            return !type.IsAbstract && typeof(IContentSerializer).IsAssignableFrom(type);
        }

        static bool HasSerializerAttribute(Type type)
        {
            return Attribute.IsDefined(type, typeof(ContentSerializerAttribute));
        }

        static ContentSerializerAttribute GetSerializerAttribute(Type type)
        {
            return Attribute.GetCustomAttribute(type, typeof(ContentSerializerAttribute)) as ContentSerializerAttribute;
        }
    }
}
