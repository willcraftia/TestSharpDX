#region Using

using System;

using D3D11RenderTargetView = SharpDX.Direct3D11.RenderTargetView;
using D3D11RenderTargetViewDescription = SharpDX.Direct3D11.RenderTargetViewDescription;
using D3D11RenderTargetViewDimension = SharpDX.Direct3D11.RenderTargetViewDimension;

#endregion

namespace Libra.Graphics
{
    public sealed class RenderTargetView : ResourceView
    {
        internal D3D11RenderTargetView D3D11RenderTargetView
        {
            get { return D3D11ResourceView as D3D11RenderTargetView; }
        }

        internal DepthStencilView DepthStencilView { get; private set; }

        public RenderTargetView(RenderTarget renderTarget)
            : this(renderTarget, false)
        {
        }

        internal RenderTargetView(RenderTarget renderTarget, bool resourceResponsibility)
            : base(renderTarget, CreateD3D11RenderTargetView(renderTarget), resourceResponsibility)
        {
            if (renderTarget.DepthStencil != null)
            {
                DepthStencilView = new DepthStencilView(renderTarget.DepthStencil);
            }
        }

        public static RenderTargetView CreateRenderTargetView(Device device, int width, int height,
            bool mipMap = false, SurfaceFormat format = SurfaceFormat.Color,
            DepthFormat depthFormat = DepthFormat.None, int multiSampleCount = 1,
            ResourceUsage resourceUsage = ResourceUsage.Default,
            RenderTargetUsage renderTargetUsage = RenderTargetUsage.Discard)
        {
            var renderTarget = new RenderTarget(device, width, height,
                mipMap, format, depthFormat, multiSampleCount, resourceUsage, renderTargetUsage);

            return new RenderTargetView(renderTarget, true);
        }

        static D3D11RenderTargetView CreateD3D11RenderTargetView(RenderTarget renderTarget)
        {
            var d3dDevice = renderTarget.Device.D3D11Device;
            var d3d11Texture2D = renderTarget.D3D11Texture2D;
            var d3d11Texture2DDescription = d3d11Texture2D.Description;

            var description = new D3D11RenderTargetViewDescription
            {
                Format = d3d11Texture2DDescription.Format,
                Texture2D =
                {
                    MipSlice = 0
                }
            };

            if (1 < d3d11Texture2DDescription.SampleDescription.Count)
            {
                description.Dimension = D3D11RenderTargetViewDimension.Texture2DMultisampled;
            }
            else
            {
                description.Dimension = D3D11RenderTargetViewDimension.Texture2D;
            }

            return new D3D11RenderTargetView(d3dDevice, d3d11Texture2D, description);
        }

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
