#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class DeviceContext : IDisposable
    {
        public event EventHandler Disposing;

        public abstract IDevice Device { get; }

        public abstract bool Deferred { get; }

        public abstract InputAssemblerStage InputAssemblerStage { get; }

        public abstract VertexShaderStage VertexShaderStage { get; }

        public abstract RasterizerStage RasterizerStage { get; }

        public abstract PixelShaderStage PixelShaderStage { get; }

        public abstract OutputMergerStage OutputMergerStage { get; }

        public abstract void Draw(int vertexCount, int startVertexLocation = 0);

        public abstract void GetData<T>(Resource resource, int level, T[] data, int startIndex, int elementCount) where T : struct;

        public void SetData<T>(Resource resource, params T[] data) where T : struct
        {
            SetData(resource, data, 0, data.Length);
        }

        public abstract void SetData<T>(Resource resource, T[] data, int startIndex, int elementCount) where T : struct;

        protected virtual void OnDisposing(object sender, EventArgs e)
        {
            if (Disposing != null)
                Disposing(sender, e);
        }

        #region IDisposable

        public bool IsDisposed { get; private set; }

        ~DeviceContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void DisposeOverride(bool disposing) { }

        void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            OnDisposing(this, EventArgs.Empty);

            DisposeOverride(disposing);

            IsDisposed = true;
        }

        #endregion
    }
}
