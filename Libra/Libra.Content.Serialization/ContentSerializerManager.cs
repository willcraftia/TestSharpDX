#region Using

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace Libra.Content.Serialization
{
    // 型やアセンブリからシリアライザを検索して登録する機能は、
    // IContentSerializer かつ ContentSerializerAttribute が設定されていることが条件。

    public sealed class ContentSerializerManager
    {
        Dictionary<string, IContentSerializer> serializerMap;

        public IContentSerializer this[string name]
        {
            get { return serializerMap[name]; }
            set { serializerMap[name] = value; }
        }

        public ContentSerializerManager()
        {
            serializerMap = new Dictionary<string, IContentSerializer>();
        }

        public bool Contains(string name)
        {
            return serializerMap.ContainsKey(name);
        }

        public void Add(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            if (!IsSerializerType(type))
                throw new ArgumentException("Type is not a IContentSerializer: " + type, "type");

            var serializerAttribute = GetSerializerAttribute(type);
            if (serializerAttribute == null)
                throw new ArgumentException("Type does not have ContentSerializerAttribute: " + type, "type");

            var serializer = Activator.CreateInstance(type) as IContentSerializer;

            serializerMap[type.AssemblyQualifiedName] = serializer;
            serializerMap[type.Name] = serializer;
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
                if (!IsSerializerType(type))
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

        public bool Remove(string name)
        {
            return serializerMap.Remove(name);
        }

        public void Clear()
        {
            serializerMap.Clear();
        }

        static bool IsSerializerType(Type type)
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
