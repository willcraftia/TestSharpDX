#region Using

using System;
using Libra.Graphics;

#endregion

namespace Libra.Xnb
{
    public sealed class XnbManager : IDisposable
    {
        Felis.Xnb.ContentManager entity;

        IDevice device;

        public string RootDirectory
        {
            get { return entity.RootDirectory; }
            set { entity.RootDirectory = value; }
        }

        public XnbManager(IDevice device)
        {
            if (device == null) throw new ArgumentNullException("device");

            this.device = device;

            entity = new Felis.Xnb.ContentManager(device);
            InitializeEntity();
        }

        void InitializeEntity()
        {
            entity.TypeReaderManager.RegisterStandardTypeReaders();

            RegisterTypeBuilder<Vector3Builder>();
            RegisterTypeBuilder<RectangleBuilder>();
            RegisterTypeBuilder<MatrixBuilder>();
            RegisterTypeBuilder<BoundingSphereBuilder>();
            RegisterTypeBuilder<VertexBufferBuilder>();
            RegisterTypeBuilder<VertexDeclarationBuilder>();
            RegisterTypeBuilder<IndexBufferBuilder>();
            RegisterTypeBuilder<BasicEffectBuilder>();
            RegisterTypeBuilder<ModelBuilder>();
            RegisterTypeBuilder<Texture2DBuilder>();
            RegisterTypeBuilder<SpriteFontBuilder>();
        }

        void RegisterTypeBuilder<T>() where T : Felis.Xnb.TypeBuilder, new()
        {
            entity.TypeReaderManager.RegisterTypeBuilder<T>();
        }

        public T Load<T>(string assetName, DeviceContext context = null)
        {
            return entity.Load<T>(assetName, context ?? device.ImmediateContext);
        }

        public void Unload()
        {
            entity.Unload();
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
