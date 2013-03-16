#region Using

using System;
using Libra.Graphics;

#endregion

namespace Libra.Content
{
    public sealed class ContentLoaderFactory
    {
        public IDevice Device { get; private set; }

        public ContentTypeReaderManager TypeReaders { get; private set; }

        public string RootDirectory { get; set; }

        public ContentLoaderFactory(IDevice device)
        {
            if (device == null) throw new ArgumentNullException("device");

            Device = device;

            TypeReaders = new ContentTypeReaderManager();
        }

        public ContentLoaderFactory(IDevice device, AppDomain appDomain)
            : this(device)
        {
            if (appDomain == null) throw new ArgumentNullException("appDomain");

            FindAndAddAllFrom(appDomain);
        }

        public void FindAndAddAllFrom(AppDomain appDomain)
        {
            TypeReaders.FindAndAddFrom(appDomain);
        }

        public ContentLoader CreateLoader(DeviceContext context = null)
        {
            return new ContentLoader(this, context);
        }
    }
}
