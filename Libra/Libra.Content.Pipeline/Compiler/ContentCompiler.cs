#region Using

using System;
using System.Collections.Generic;
using System.IO;
using Libra.Content.Pipeline.Processors;
using Libra.Content.Serialization;

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
        // sourcePath は出力先ファイルの命名に利用されるため、
        // ファイルへ永続化する場合には sourcePath の指定が必須となる (ストリーム指定ではファイル名を解決できない)。

        public string Compile<TSerializer, TProcessor>(string sourcePath, Properties processorProperties = null)
            where TSerializer : IContentSerializer, new()
            where TProcessor : IContentProcessor, new()
        {
            var serializer = new TSerializer();

            var processor = new TProcessor();
            PopulateProperties(processor, processorProperties);

            return Compile(sourcePath, serializer, processor);
        }

        // シリアライザとプロセッサの名前指定は、ビルド ファイル等からの呼び出しを想定。

        public string Compile(
            string sourcePath,
            string serializerName,
            string processorName, Properties processorProperties = null)
        {
            if (string.IsNullOrEmpty(serializerName))
                throw new ArgumentException("serializerName must be not null/empty.", "serializerName");
            if (string.IsNullOrEmpty(processorName))
                throw new ArgumentException("processorName must be not null/empty.", "processorName");

            var serializer = GetSerializer(serializerName);

            var processor = CreateProcessor(processorName, processorProperties);

            return Compile(sourcePath, serializer, processor);
        }

        public string Compile(string sourcePath, IContentSerializer serializer, IContentProcessor processor)
        {
            if (string.IsNullOrEmpty(sourcePath)) throw new ArgumentException("sourcePath must be not null/empty.", "sourcePath");
            if (serializer == null) throw new ArgumentNullException("serializer");
            if (processor == null) throw new ArgumentNullException("processor");

            // ソースのオブジェクト化。
            var source = DeserializeSource(sourcePath, serializer);

            // 加工。
            var artifact = processor.Process(source);

            // バイナリ永続化。
            return Write(sourcePath, artifact);
        }

        // ストリーム指定バージョンは、呼び出し元が入力元や出力先の解決を担う。

        public void Compile<TSerializer, TProcessor>(string sourcePath, Stream outputStream,
            Properties processorProperties = null)
            where TSerializer : IContentSerializer, new()
            where TProcessor : IContentProcessor, new()
        {
            var serializer = new TSerializer();

            var processor = new TProcessor();
            PopulateProperties(processor, processorProperties);

            Compile(sourcePath, serializer, processor);
        }

        public void Compile<TSerializer, TProcessor>(Stream inputStream, Stream outputStream, Properties processorProperties = null)
            where TSerializer : IContentSerializer, new()
            where TProcessor : IContentProcessor, new()
        {
            var serializer = new TSerializer();

            var processor = new TProcessor();
            PopulateProperties(processor, processorProperties);

            Compile(inputStream, outputStream, serializer, processor);
        }

        public void Compile(string sourcePath, Stream outputStream,
            string serializerName, string processorName, Properties processorProperties = null)
        {
            if (string.IsNullOrEmpty(serializerName))
                throw new ArgumentException("serializerName must be not null/empty.", "serializerName");
            if (string.IsNullOrEmpty(processorName))
                throw new ArgumentException("processorName must be not null/empty.", "processorName");

            var serializer = GetSerializer(serializerName);

            var processor = CreateProcessor(processorName, processorProperties);

            Compile(sourcePath, outputStream, serializer, processor);
        }

        public void Compile(Stream inputStream, Stream outputStream,
            string serializerName, string processorName, Properties processorProperties = null)
        {
            if (string.IsNullOrEmpty(serializerName))
                throw new ArgumentException("serializerName must be not null/empty.", "serializerName");
            if (string.IsNullOrEmpty(processorName))
                throw new ArgumentException("processorName must be not null/empty.", "processorName");

            var serializer = GetSerializer(serializerName);

            var processor = CreateProcessor(processorName, processorProperties);

            Compile(inputStream, outputStream, serializer, processor);
        }

        public void Compile(string sourcePath, Stream outputStream, IContentSerializer serializer, IContentProcessor processor)
        {
            if (string.IsNullOrEmpty(sourcePath)) throw new ArgumentException("sourcePath must be not null/empty.", "sourcePath");

            using (var inputStream = OpenSourceStream(sourcePath))
            {
                Compile(inputStream, outputStream, serializer, processor);
            }
        }

        public void Compile(Stream inputStream, Stream outputStream, IContentSerializer serializer, IContentProcessor processor)
        {
            if (inputStream == null) throw new ArgumentNullException("inputStream");
            if (outputStream == null) throw new ArgumentNullException("outputStream");
            if (serializer == null) throw new ArgumentNullException("serializer");
            if (processor == null) throw new ArgumentNullException("processor");

            // ソースのオブジェクト化。
            var source = serializer.Deserialize(inputStream);

            // 加工。
            var artifact = processor.Process(source);

            // バイナリ永続化。
            // クラス外部から指定されたストリームであるため leaveOpen = true とし、
            // ストリームの破棄は呼び出し元の責務とする。
            Write(outputStream, artifact, true);
        }

        Stream OpenSourceStream(string path)
        {
            var targetPath = (factory.SourceRootDirectory == null) ? path : Path.Combine(factory.SourceRootDirectory, path);

            return File.OpenRead(targetPath);
        }

        object DeserializeSource(string path, IContentSerializer serializer)
        {
            using (var stream = OpenSourceStream(path))
            {
                return serializer.Deserialize(stream);
            }
        }

        IContentSerializer GetSerializer(string name)
        {
            return factory.Serializers[name];
        }

        IContentProcessor CreateProcessor(string name, Properties propertyMap)
        {
            var type = factory.ProcessorTypes[name];
            var processor = Activator.CreateInstance(type) as IContentProcessor;

            PopulateProperties(processor, propertyMap);

            return processor;
        }

        void PopulateProperties(IContentProcessor processor, Properties propertyMap)
        {
            if (propertyMap != null && propertyMap.Count != 0)
            {
                var type = processor.GetType();

                foreach (var propertyName in propertyMap.Keys)
                {
                    var property = type.GetProperty(propertyName);
                    property.SetValue(processor, propertyMap[propertyName], null);
                }
            }
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
            if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            using (var stream = File.Create(outputPath))
            {
                Write(stream, content);
            }

            return outputPath;
        }

        void Write(Stream stream, object content, bool leaveOpen = false)
        {
            // 内部管理のストリームならば leaveOpen = false。
            // クラス利用側が指定したストリームならば leaveOpen = true。

            using (var writer = new ContentWriter(factory.TypeWriters, stream, leaveOpen))
            {
                writer.WriteObject(content);
                writer.Flush();
            }
        }
    }
}
