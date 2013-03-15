#region Using

using System;
using System.Collections.Generic;
using System.IO;
using Libra.Content.Pipeline.Processors;

#endregion

namespace Libra.Content.Pipeline.Compiler
{
    public sealed class ContentCompiler
    {
        ContentCompilerFactory factory;

        internal ContentCompiler(ContentCompilerFactory factory)
        {
            if (factory == null) throw new ArgumentNullException("factory");

            this.factory = factory;
        }

        public string Compile(string sourcePath, IContentProcessor processor, string outputDirectory = null)
        {
            if (string.IsNullOrEmpty(sourcePath)) throw new ArgumentException("sourcePath must be not null/empty.", "sourcePath");
            if (processor == null) throw new ArgumentNullException("processor");

            // ソースのオブジェクト化。
            var source = DeserializeSource(sourcePath);

            // 加工。
            var artifact = processor.Process(source);

            // バイナリ永続化。
            return Write(sourcePath, outputDirectory, artifact);
        }

        public string Compile(
            string contentDescriptionPath,
            string processorName, Dictionary<string, string> processorProperties = null,
            string outputDirectory = null)
        {
            if (string.IsNullOrEmpty(processorName))
                throw new ArgumentException("processorName must be not null/empty.", "processorName");

            var processor = CreateProcessor(processorName, processorProperties);

            return Compile(contentDescriptionPath, processor, outputDirectory);
        }

        object DeserializeSource(string path)
        {
            var extension = Path.GetExtension(path);
            var serializer = factory.Serializers[extension];

            using (var stream = File.OpenRead(path))
            {
                return serializer.Deserialize(stream);
            }
        }

        IContentProcessor CreateProcessor(string name, Dictionary<string, string> propertyMap)
        {
            var type = factory.ProcessorTypes[name];
            var processor = Activator.CreateInstance(type) as IContentProcessor;

            if (propertyMap != null && propertyMap.Count != 0)
            {
                foreach (var propertyName in propertyMap.Keys)
                {
                    var property = type.GetProperty(propertyName);
                    property.SetValue(processor, propertyMap[propertyName], null);
                }
            }

            return processor;
        }

        string Write(string contentDescriptionPath, string outputDirectory, object content)
        {
            // .ccb (Compiled Content Binary)
            var filename = Path.GetFileNameWithoutExtension(contentDescriptionPath) + ".ccb";

            string outputPath;
            if (string.IsNullOrEmpty(outputDirectory))
            {
                outputPath = filename;
            }
            else
            {
                outputPath = Path.Combine(outputDirectory, filename);
            }

            using (var stream = File.Create(outputPath))
            {
                Write(stream, content);
            }

            return outputPath;
        }

        void Write(Stream stream, object content)
        {
            using (var writer = new ContentWriter(stream, factory.TypeWriters))
            {
                writer.WriteObject(content);
                writer.Flush();
            }
        }
    }
}
