#region Using

using System;
using System.IO;
using Libra.Graphics;

#endregion

namespace Libra.Content
{
    public sealed class ContentLoader
    {
        ContentLoaderFactory factory;

        DeviceContext context;

        internal ContentLoader(ContentLoaderFactory factory, DeviceContext context)
        {
            if (factory == null) throw new ArgumentNullException("factory");

            this.factory = factory;
            this.context = context ?? factory.Device.ImmediateContext;
        }

        public T Load<T>(string path)
        {
            if (path == null) throw new ArgumentNullException("path");

            string filePath;
            if (factory.RootDirectory == null)
            {
                filePath = path;
            }
            else
            {
                filePath = Path.Combine(factory.RootDirectory, path);
            }

            filePath += ".ccb";

            using (var stream = File.OpenRead(filePath))
            {
                return Read<T>(stream);
            }
        }

        public T Load<T>(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            return Read<T>(stream);
        }

        T Read<T>(Stream stream)
        {
            using (var reader = new ContentReader(stream, factory.TypeReaders, factory.Device, context))
            {
                return reader.ReadObject<T>();
            }
        }
    }
}
