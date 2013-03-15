#region Using

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace Libra.Content.Pipeline.Processors
{
    public sealed class ContentProcessorManager
    {
        static readonly Type ProcessorInterface = typeof(IContentProcessor);

        static readonly Type ProcessorAttributeType = typeof(ContentProcessorAttribute);

        Dictionary<string, Type> processors;

        Dictionary<string, Type> shortNameProcessors;

        public ContentProcessorManager()
        {
            processors = new Dictionary<string, Type>();
            shortNameProcessors = new Dictionary<string, Type>();
        }

        public IContentProcessor CreateProcessor(string name, Dictionary<string, string> properties)
        {
            var type = ResolveProcessor(name);
            var processor = Activator.CreateInstance(type) as IContentProcessor;

            Populate(processor, properties);

            return processor;
        }

        public void AddProcessor(Type type)
        {
            if (!IsProcessor(type))
                throw new ArgumentException("Type is not a content processor.", "type");

            processors[type.AssemblyQualifiedName] = type;
            shortNameProcessors[type.Name] = type;
        }

        public void AddPrcessors()
        {
            AddPrcessors(AppDomain.CurrentDomain);
        }

        public void AddPrcessors(AppDomain appDomain)
        {
            foreach (var assembly in appDomain.GetAssemblies())
            {
                AddPrcessors(assembly);
            }
        }

        public void AddPrcessors(Assembly assembly)
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
                if (!IsProcessor(type))
                    continue;

                // 探索からの自動登録では ContentProcessorAttribute が明示されたクラスのみが対象。
                if (!HasProcessorAttribute(type))
                    continue;

                AddProcessor(type);
            }
        }

        Type ResolveProcessor(string name)
        {
            Type type;

            // 完全修飾名で見つかるか否か。
            if (processors.TryGetValue(name, out type))
                return type;

            // 短縮名で見つかるか否か。
            if (shortNameProcessors.TryGetValue(name, out type))
                return type;

            throw new InvalidOperationException("Processor not found: " + name);
        }

        static bool IsProcessor(Type type)
        {
            return ProcessorInterface.IsAssignableFrom(type);
        }

        static bool HasProcessorAttribute(Type type)
        {
            return Attribute.IsDefined(type, ProcessorAttributeType);
        }

        static void Populate(IContentProcessor processor, Dictionary<string, string> propertyMap)
        {
            var type = processor.GetType();

            foreach (var propertyName in propertyMap.Keys)
            {
                var property = type.GetProperty(propertyName);
                property.SetValue(processor, propertyMap[propertyName], null);
            }
        }
    }
}
