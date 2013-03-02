#region Using

using System;

using D3D11ResourceView = SharpDX.Direct3D11.ResourceView;

#endregion

namespace Libra.Graphics.SharpDX
{
    public class SdxResourceView : IResourceView
    {
        bool resourceResponsibility;

        public IResource Resource { get; private set; }

        public D3D11ResourceView D3D11ResourceView { get; private set; }

        protected SdxResourceView(D3D11ResourceView d3d11ResourceView, SdxResource resource, bool resourceResponsibility)
        {
            if (d3d11ResourceView == null) throw new ArgumentNullException("d3d11ResourceView");
            if (resource == null) throw new ArgumentNullException("resource");

            D3D11ResourceView = d3d11ResourceView;
            Resource = resource;
            this.resourceResponsibility = resourceResponsibility;
        }

        #region IDisposable

        bool disposed;

        ~SdxResourceView()
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
            if (disposed) return;

            DisposeOverride(disposing);

            // resourceResponsibility = true でインスタンス化している場合は、
            // ここでリソース破棄の責務を負う。
            if (resourceResponsibility)
                D3D11ResourceView.Dispose();

            disposed = true;
        }

        #endregion
    }
}
