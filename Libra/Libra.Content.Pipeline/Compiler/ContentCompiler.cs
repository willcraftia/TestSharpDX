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

        // sourcePath には、factory の SourceRootDirectory からの相対パスを指定。

        public string Compile(string sourcePath, IContentProcessor processor)
        {
            if (string.IsNullOrEmpty(sourcePath)) throw new ArgumentException("sourcePath must be not null/empty.", "sourcePath");
            if (processor == null) throw new ArgumentNullException("processor");

            // ソースのオブジェクト化。
            var source = DeserializeSource(sourcePath);

            // 加工。
            var artifact = processor.Process(source);

            // バイナリ永続化。
            return Write(sourcePath, artifact);
        }

        public string Compile(
            string sourcePath,
            string processorName, Dictionary<string, object> processorProperties = null)
        {
            if (string.IsNullOrEmpty(processorName))
                throw new ArgumentException("processorName must be not null/empty.", "processorName");

            var processor = CreateProcessor(processorName, processorProperties);

            return Compile(sourcePath, processor);
        }

        object DeserializeSource(string path)
        {
            var extension = Path.GetExtension(path);
            var serializer = factory.Serializers[extension];

            var targetPath = (factory.SourceRootDirectory == null) ? path : Path.Combine(factory.SourceRootDirectory, path);

            using (var stream = File.OpenRead(targetPath))
            {
                return serializer.Deserialize(stream);
            }
        }

        IContentProcessor CreateProcessor(string name, Dictionary<string, object> propertyMap)
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

        string Write(string sourcePath, object content)
        {
            // .ccb (Compiled Content Binary)
            var filename = Path.GetFileNameWithoutExtension(sourcePath) + ".ccb";

            var sourceDirectoryPath = Path.GetDirectoryName(sourcePath);

            var filePath = Path.Combine(sourceDirectoryPath, filename);

            string outputPath;
            if (factory.OutputRootDirectory == null)
            {
                outputPath = filePath;
            }
            else
            {
                outputPath = Path.Combine(factory.OutputRootDirectory, filePath);
            }

            var outputDirectory = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

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
