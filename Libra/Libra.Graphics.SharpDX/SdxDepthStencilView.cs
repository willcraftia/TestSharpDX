#region Using

using System;

using D3D11DepthStencilView = SharpDX.Direct3D11.DepthStencilView;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxDepthStencilView : SdxResourceView, IDepthStencilView
    {
        public D3D11DepthStencilView D3D11DepthStencilView
        {
            get { return D3D11ResourceView as D3D11DepthStencilView; }
        }

        public SdxDepthStencilView(
            D3D11DepthStencilView d3d11DepthStencilView,
            SdxDepthStencil depthStencil,
            bool resourceResponsibility = false)
            : base(d3d11DepthStencilView, depthStencil, resourceResponsibility)
        {
        }
    }
}
