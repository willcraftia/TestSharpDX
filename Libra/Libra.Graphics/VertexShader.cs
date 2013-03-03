#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class VertexShader : IDisposable
    {
        protected VertexShader() { }

        public abstract void Initialize(byte[] shaderBytecode);

        #region IDisposable

        public bool IsDisposed { get; private set; }

        ~VertexShader()
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

            DisposeOverride(disposing);

            IsDisposed = true;
        }

        #endregion
    }
}
