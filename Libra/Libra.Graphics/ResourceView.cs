#region Using

using System;

using D3D11ResourceView = SharpDX.Direct3D11.ResourceView;

#endregion

namespace Libra.Graphics
{
    public class ResourceView : IDisposable
    {
        public event EventHandler Disposing;

        bool resourceResponsibility;

        public Resource Resource { get; private set; }

        internal D3D11ResourceView D3D11ResourceView { get; private set; }

        internal ResourceView(Resource resource, D3D11ResourceView d3d11ResourceView, bool resourceResponsibility)
        {
            Resource = resource;
            D3D11ResourceView = d3d11ResourceView;
            this.resourceResponsibility = resourceResponsibility;
        }

        #region IDisposable

        public bool IsDisposed { get; private set; }

        ~ResourceView()
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

            if (Disposing != null)
                Disposing(this, EventArgs.Empty);

            DisposeOverride(disposing);

            // resourceResponsibility = true でインスタンス化している場合は、
            // ここでリソース破棄の責務を負う。
            if (resourceResponsibility)
                D3D11ResourceView.Dispose();

            IsDisposed = true;
        }

        #endregion
    }
}
