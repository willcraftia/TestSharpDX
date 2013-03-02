#region Using

using System;

using D3D11RenderTargetView = SharpDX.Direct3D11.RenderTargetView;
using D3D11RenderTargetViewDescription = SharpDX.Direct3D11.RenderTargetViewDescription;
using D3D11RenderTargetViewDimension = SharpDX.Direct3D11.RenderTargetViewDimension;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxRenderTargetView : SdxResourceView, IRenderTargetView
    {
        public IDepthStencilView DepthStencilView { get; private set; }

        public D3D11RenderTargetView D3D11RenderTargetView
        {
            get { return D3D11ResourceView as D3D11RenderTargetView; }
        }

        public SdxRenderTargetView(
            D3D11RenderTargetView d3d11RenderTargetView,
            SdxRenderTarget renderTarget,
            bool resourceResponsibility = false,
            IDepthStencilView depthStencilView = null)
            : base(d3d11RenderTargetView, renderTarget, resourceResponsibility)
        {
            DepthStencilView = depthStencilView;
        }

        //public static SdxRenderTargetView CreateRenderTargetView(SdxDevice device, int width, int height,
        //    bool mipMap = false, SurfaceFormat format = SurfaceFormat.Color,
        //    DepthFormat depthFormat = DepthFormat.None, int multiSampleCount = 1,
        //    ResourceUsage resourceUsage = ResourceUsage.Default,
        //    RenderTargetUsage renderTargetUsage = RenderTargetUsage.Discard)
        //{
        //    var renderTarget = new SdxRenderTarget(device, width, height,
        //        mipMap, format, depthFormat, multiSampleCount, resourceUsage, renderTargetUsage);

        //    return new SdxRenderTargetView(renderTarget, true);
        //}

        protected override void DisposeOverride(bool disposing)
        {
            if (disposing)
            {
                if (DepthStencilView != null)
                    DepthStencilView.Dispose();

                D3D11RenderTargetView.Dispose();
            }

            base.DisposeOverride(disposing);
        }
    }
}
