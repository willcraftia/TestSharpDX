#region Using

using System;
using System.Collections.Generic;
using System.IO;

#endregion

namespace Felis.Xnb
{
    public class ContentManager : IDisposable
    {
        string rootDirectory;

        Dictionary<string, object> assetByName;

        List<IDisposable> disposableObjects;

        public IServiceProvider ServiceProvider { get; private set; }

        public TypeReaderManager TypeReaderManager { get; private set; }

        public string RootDirectory
        {
            get { return rootDirectory; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                rootDirectory = value;
            }
        }

        public ContentManager(IServiceProvider serviceProvider)
            : this(serviceProvider, string.Empty)
        {
        }

        public ContentManager(IServiceProvider serviceProvider, string rootDirectory)
        {
            if (serviceProvider == null) throw new ArgumentNullException("serviceProvider");
            if (rootDirectory == null) throw new ArgumentNullException("rootDirectory");

            ServiceProvider = serviceProvider;
            this.rootDirectory = rootDirectory;

            TypeReaderManager = new TypeReaderManager(this);
            assetByName = new Dictionary<string, object>();
            disposableObjects = new List<IDisposable>();
        }

        public T Load<T>(string assetName, object deviceContext = null)
        {
            object asset;
            if (assetByName.TryGetValue(assetName, out asset))
                return (T) asset;

            asset = ReadAsset<T>(assetName, deviceContext, null);

            assetByName[assetName] = asset;

            return (T) asset;
        }

        public void Unload()
        {
            for (int i = 0; i < disposableObjects.Count; i++)
                disposableObjects[i].Dispose();

            disposableObjects.Clear();
            assetByName.Clear();
        }

        protected T ReadAsset<T>(string assetName, object deviceContext, Action<IDisposable> recordDisposableObject)
        {
            var filename = assetName + ".xnb";
            var filePath = Path.Combine(rootDirectory, filename);

            using (var stream = File.OpenRead(filePath))
            {
                using (var reader = new ContentReader(stream, assetName, this, deviceContext, recordDisposableObject))
                {
                    return (T) reader.ReadXnb();
                }
            }
        }

        internal void RecordDisposableObject(IDisposable disposable)
        {
            disposableObjects.Add(disposable);
        }

        #region IDisposable

        bool disposed;

        ~ContentManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                Unload();
            }

            disposed = true;
        }

        #endregion
    }
}
