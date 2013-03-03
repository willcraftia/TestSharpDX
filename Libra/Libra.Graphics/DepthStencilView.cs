#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class DepthStencilView : IDisposable
    {
        public DepthStencil DepthStencil { get; private set; }

        protected DepthStencilView() { }

        public void Initialize(DepthStencil depthStencil)
        {
            if (depthStencil == null) throw new ArgumentNullException("depthStencil");

            DepthStencil = depthStencil;

            Initialize();
        }

        protected abstract void Initialize();

        #region IDisposable

        public bool IsDisposed { get; private set; }

        ~DepthStencilView()
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
