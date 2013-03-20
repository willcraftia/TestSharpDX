#region Using

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Libra.Graphics;

#endregion

namespace Libra.Content.Xnb
{
    public class XnbManager : IDisposable
    {
        string rootDirectory;

        Dictionary<string, object> assetByName;

        List<IDisposable> disposableObjects;

        public IDevice Device { get; private set; }

        public XnbTypeReaderManager TypeReaderManager { get; private set; }

        public string RootDirectory
        {
            get { return rootDirectory; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                rootDirectory = value;
            }
        }

        public XnbManager(IDevice device)
        {
            if (device == null) throw new ArgumentNullException("device");

            Device = device;

            rootDirectory = string.Empty;
            TypeReaderManager = new XnbTypeReaderManager();
            assetByName = new Dictionary<string, object>();
            disposableObjects = new List<IDisposable>();
        }

        public T Load<T>(string assetName, DeviceContext context = null)
        {
            object asset;
            if (assetByName.TryGetValue(assetName, out asset))
                return (T) asset;

            asset = ReadAsset<T>(assetName, context, null);

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

        protected T ReadAsset<T>(string assetName, DeviceContext context, Action<IDisposable> recordDisposableObject)
        {
            var filename = assetName + ".xnb";
            var filePath = Path.Combine(rootDirectory, filename);

            using (var stream = File.OpenRead(filePath))
            {
                using (var reader = new XnbReader(stream, assetName, this, recordDisposableObject, context))
                {
                    return reader.ReadXnb<T>();
                }
            }
        }

        internal void RecordDisposableObject(IDisposable disposable)
        {
            disposableObjects.Add(disposable);
        }

        #region IDisposable

        bool disposed;

        ~XnbManager()
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
