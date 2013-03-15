#region Using

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace Libra.Content.Pipeline.Processors
{
    // Add は、型の完全修飾名および省略名をプロセッサ名として登録。
    // インデクサは任意の名前で登録。
    // いずれも、型が IContentProcessor かつ ContentProcessorAttribute を設定している事が条件。

    public sealed class ContentProcessorTypeCollection
    {
        Dictionary<string, Type> processorMap;

        public Type this[string name]
        {
            get { return processorMap[name]; }
            set
            {
                if (!IsProcessorType(value))
                    throw new ArgumentException("Type is not a IContentProcessor: " + value, "value");
                if (HasProcessorAttribute(value))
                    throw new ArgumentException("Type does not have ContentProcessorAttribute: " + value, "value");

                processorMap[name] = value;
            }
        }

        public ContentProcessorTypeCollection()
        {
            processorMap = new Dictionary<string, Type>();
        }

        public bool Contains(string name)
        {
            return processorMap.ContainsKey(name);
        }

        public void Add(Type type)
        {
            if (!IsProcessorType(type))
                throw new ArgumentException("Type is not a IContentProcessor: " + type, "type");
            if (!HasProcessorAttribute(type))
                throw new ArgumentException("Type does not have ContentProcessorAttribute: " + type, "type");

            processorMap[type.AssemblyQualifiedName] = type;
            processorMap[type.Name] = type;
        }

        public void FindAndAddFrom(Assembly assembly)
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
                if (!IsProcessorType(type))
                    continue;

                if (!HasProcessorAttribute(type))
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
            return processorMap.Remove(name);
        }

        public void Clear()
        {
            processorMap.Clear();
        }

        static bool IsProcessorType(Type type)
        {
            return typeof(IContentProcessor).IsAssignableFrom(type);
        }

        static bool HasProcessorAttribute(Type type)
        {
            return Attribute.IsDefined(type, typeof(ContentProcessorAttribute));
        }
    }
}
