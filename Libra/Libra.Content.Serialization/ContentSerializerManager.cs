#region Using

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace Libra.Content.Serialization
{
    public sealed class ContentSerializerManager
    {
        static readonly Type SerializerInterfaceType = typeof(IContentSerializer);

        static readonly Type SerializerAttributeType = typeof(ContentSerializerAttribute);

        // キー: 拡張子 (ドット付き)
        Dictionary<string, IContentSerializer> serializers;

        public ContentSerializerManager()
        {
            serializers = new Dictionary<string, IContentSerializer>();
        }

        public bool ContainsSerializer(string extension)
        {
            return serializers.ContainsKey(extension);
        }

        public IContentSerializer GetSerializer(string extension)
        {
            return serializers[extension];
        }

        public void AddSerializer(Type type, params string[] extensions)
        {
            if (!IsSerializer(type))
                throw new ArgumentException("Type is not a content serializer.", "type");

            var serializer = Activator.CreateInstance(type) as IContentSerializer;

            AddSerializer(serializer, extensions);
        }

        public void AddSerializer(IContentSerializer serializer, params string[] extensions)
        {
            if (extensions == null || extensions.Length == 0)
            {
                var serializerAttribute = GetSerializerAttribute(serializer.GetType());
                if (serializerAttribute == null)
                    throw new ArgumentException("ContentSerializerAttribute not found: " + serializer.GetType());

                extensions = serializerAttribute.Extensions;
            }

            if (extensions == null || extensions.Length == 0)
                throw new InvalidOperationException("Can not resolve extensions.");

            // 属性による拡張子指定は無視し、引数による指定に従う。
            foreach (var extension in extensions)
            {
                serializers[extension] = serializer;
            }
        }

        public void AddSerializers()
        {
            AddSerializers(AppDomain.CurrentDomain);
        }

        public void AddSerializers(AppDomain appDomain)
        {
            foreach (var assembly in appDomain.GetAssemblies())
            {
                AddSerializers(assembly);
            }
        }

        public void AddSerializers(Assembly assembly)
        {
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

                // 探索からの自動登録では ContentSerializerAttribute が明示されたクラスのみが対象。
                if (!HasSerializerAttribute(type))
                    continue;

                var serializer = Activator.CreateInstance(type) as IContentSerializer;
                AddSerializer(serializer);
            }
        }

        static bool IsSerializer(Type type)
        {
            return !type.IsAbstract && SerializerInterfaceType.IsAssignableFrom(type);
        }

        static bool HasSerializerAttribute(Type type)
        {
            return Attribute.IsDefined(type, SerializerAttributeType);
        }

        static ContentSerializerAttribute GetSerializerAttribute(Type type)
        {
            return Attribute.GetCustomAttribute(type, SerializerAttributeType) as ContentSerializerAttribute;
        }
    }
}
