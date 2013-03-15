#region Using

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using Libra.Content.Pipeline.Processors;
using Libra.Content.Pipeline.Compiler;
using Libra.Content.Serialization;

#endregion

namespace Libra.Content.Pipeline.Build
{
    public sealed class ContentProject
    {
        static DataContractJsonSerializer descriptionSerializer;

        ContentProjectDescription description;

        ContentSerializerManager serializerManager;

        ContentProcessorManager processorManager;

        ContentCompiler compiler;

        static ContentProject()
        {
            descriptionSerializer = new DataContractJsonSerializer(typeof(ContentProjectDescription));
        }

        public ContentProject()
        {
            serializerManager = new ContentSerializerManager();
            processorManager = new ContentProcessorManager();
            compiler = new ContentCompiler();
        }

        public void Initialize(string descriptionPath)
        {
            var description = ReadDescription(descriptionPath);

            Initialize(description);
        }

        public void Initialize(ContentProjectDescription description)
        {
            this.description = description;

            if (description.DetectSerializers)
            {
                serializerManager.AddSerializers();
            }
            else
            {
                foreach (var serializer in description.Serializers)
                {
                    RegisterSerializer(serializer);
                }
            }

            if (description.DetectProcessors)
            {
                processorManager.AddPrcessors();
            }
            else
            {
                foreach (var processor in description.Processors)
                {
                    RegisterProcessor(processor);
                }
            }
        }

        public void Build()
        {
            foreach (var buildTarget in description.BuildTargets)
            {
                Build(buildTarget);
            }
        }

        static ContentProjectDescription ReadDescription(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return descriptionSerializer.ReadObject(stream) as ContentProjectDescription;
            }
        }

        void RegisterSerializer(ContentSerializerItem serializer)
        {
            var type = Type.GetType(serializer.AssemblyQualifiedName);
            var extensions = serializer.Extensions;

            serializerManager.AddSerializer(type, extensions);
        }

        void RegisterProcessor(ContentProcessorItem processor)
        {
            var type = Type.GetType(processor.AssemblyQualifiedName);

            processorManager.AddProcessor(type);
        }

        void Build(BuildTargetItem buildTarget)
        {
            // 定義のデシリアライズ。
            var contentDescription = DeserializeContentDescription(buildTarget.Path);

            // コンテントの生成。
            var content = CreateContent(buildTarget.ProcessorName, buildTarget.ProcessorProperties, contentDescription);

            // コンテントのシリアライズ。
            Compile(buildTarget.Path, content);
        }

        object DeserializeContentDescription(string path)
        {
            var extension = Path.GetExtension(path);
            var serializer = serializerManager.GetSerializer(extension);

            using (var stream = File.OpenRead(path))
            {
                return serializer.Deserialize(stream);
            }
        }

        object CreateContent(string processorName, Dictionary<string, string> processorProperties, object contentDescription)
        {
            var processor = processorManager.CreateProcessor(processorName, processorProperties);
            return processor.Process(contentDescription);
        }

        void Compile(string contentDescriptionPath, object content)
        {
            // .ccb (Compiled Content Binary)
            var filename = Path.GetFileNameWithoutExtension(contentDescriptionPath) + ".ccb";

            var outputPath = Path.Combine(description.OutputDirectory, filename);

            using (var stream = File.Create(outputPath))
            {
                compiler.Compile(stream, content);
            }
        }
    }
}
