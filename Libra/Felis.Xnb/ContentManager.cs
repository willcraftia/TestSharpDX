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

        public ContentManager()
        {
            rootDirectory = string.Empty;
            TypeReaderManager = new TypeReaderManager();
            assetByName = new Dictionary<string, object>();
            disposableObjects = new List<IDisposable>();
        }

        public T Load<T>(string assetName)
        {
            object asset;
            if (assetByName.TryGetValue(assetName, out asset))
                return (T) asset;

            asset = ReadAsset(assetName, null);

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

        protected object ReadAsset(string assetName, Action<IDisposable> recordDisposableObject)
        {
            var filename = assetName + ".xnb";
            var filePath = Path.Combine(rootDirectory, filename);

            using (var stream = File.OpenRead(filePath))
            {
                using (var reader = new ContentReader(stream, assetName, this, recordDisposableObject))
                {
                    return reader.ReadXnb();
                }
            }

            throw new NotImplementedException();
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
