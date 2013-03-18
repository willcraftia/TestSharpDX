#region Using

using System;
using System.Collections.Generic;
using System.IO;
using Libra.Graphics;
using Libra.Content.Pipeline.Compiler;
using Libra.Content.Pipeline.Processors;
using Libra.Content.Serialization;

#endregion

namespace Libra.Content.Pipeline.Utilities
{
    public sealed class AdhocContentLoader
    {
        public ContentCompilerFactory CompilerFactory { get; private set; }

        public ContentLoaderFactory LoaderFactory { get; private set; }

        public AdhocContentLoader(IDevice device)
            : this(new ContentCompilerFactory(), new ContentLoaderFactory(device))
        {
        }

        public AdhocContentLoader(IDevice device, AppDomain appDomain)
            : this(new ContentCompilerFactory(appDomain), new ContentLoaderFactory(device, appDomain))
        {
        }

        public AdhocContentLoader(ContentCompilerFactory compilerFactory, ContentLoaderFactory loaderFactory)
        {
            if (compilerFactory == null) throw new ArgumentNullException("compilerFactory");
            if (loaderFactory == null) throw new ArgumentNullException("loaderFactory");

            this.CompilerFactory = compilerFactory;
            this.LoaderFactory = loaderFactory;
        }

        public TOutput Load<TOutput, TSerializer, TProcessor>(string sourcePath, Properties processorProperties = null)
            where TSerializer : IContentSerializer, new()
            where TProcessor : IContentProcessor, new()
        {
            var compiler = CompilerFactory.CreateCompiler();
            var loader = LoaderFactory.CreateLoader();

            using (var outputStream = new MemoryStream())
            {
                compiler.Compile<TSerializer, TProcessor>(sourcePath, outputStream, processorProperties);

                outputStream.Position = 0;

                return loader.Load<TOutput>(outputStream);
            }
        }

        public T Load<T>(string sourcePath, string serializerName, string processorName, Properties processorProperties = null)
        {
            var compiler = CompilerFactory.CreateCompiler();
            var loader = LoaderFactory.CreateLoader();

            using (var outputStream = new MemoryStream())
            {
                compiler.Compile(sourcePath, outputStream, serializerName, processorName, processorProperties);

                outputStream.Position = 0;

                return loader.Load<T>(outputStream);
            }
        }

        public T Load<T>(string sourcePath, IContentSerializer serializer, IContentProcessor processor)
        {
            var compiler = CompilerFactory.CreateCompiler();
            var loader = LoaderFactory.CreateLoader();

            using (var outputStream = new MemoryStream())
            {
                compiler.Compile(sourcePath, outputStream, serializer, processor);

                outputStream.Position = 0;

                return loader.Load<T>(outputStream);
            }
        }
    }
}
