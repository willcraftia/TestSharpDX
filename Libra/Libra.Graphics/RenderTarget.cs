#region Using

using System;

using D3D11BindFlags = SharpDX.Direct3D11.BindFlags;
using D3D11ResourceUsage = SharpDX.Direct3D11.ResourceUsage;
using D3D11Texture2D = SharpDX.Direct3D11.Texture2D;
using DXGIFormat = SharpDX.DXGI.Format;

#endregion

namespace Libra.Graphics
{
    public sealed class RenderTarget : Texture2D
    {
        const D3D11BindFlags bindFlag = D3D11BindFlags.ShaderResource | D3D11BindFlags.RenderTarget;

        public DepthFormat DepthFormat { get; private set; }

        public RenderTargetUsage RenderTargetUsage { get; private set; }

        internal DepthStencil DepthStencil { get; private set; }

        public RenderTarget(Device device, int width, int height,
            bool mipMap = false, SurfaceFormat format = SurfaceFormat.Color,
            DepthFormat depthFormat = DepthFormat.None, int multiSampleCount = 1,
            ResourceUsage resourceUsage = ResourceUsage.Default,
            RenderTargetUsage renderTargetUsage = RenderTargetUsage.Discard)
            : this(device,
                CreateD3D11Texture2D(device, width, height, mipMap, format, multiSampleCount, resourceUsage, bindFlag),
                depthFormat, renderTargetUsage)
        {
            // TODO
            //
            // RenderTargetUsage.Discard の場合、同一設定のテクスチャを共有できる。
            // デバイスでテクスチャの参照を管理し、
            // テクスチャの IUnknown.AddRef/Release で参照カウンタを制御。
            // Release で参照カウンタが 0 になる場合、デバイスでテクスチャを破棄。
            //
            // 同様の処理を深度ステンシルでも行う。
        }

        internal RenderTarget(Device device, D3D11Texture2D d3d11Texture2D, DepthFormat depthFormat,
            RenderTargetUsage renderTargetUsage)
            : base(device, d3d11Texture2D)
        {
            if (d3d11Texture2D.Description.Usage != D3D11ResourceUsage.Default &&
                d3d11Texture2D.Description.Usage != D3D11ResourceUsage.Staging)
                throw new ArgumentException("ResourceUsage.Default or Staging required.", "usage");

            DepthFormat = depthFormat;
            RenderTargetUsage = renderTargetUsage;

            if (depthFormat != DepthFormat.None)
            {
                DepthStencil = new DepthStencil(device, Width, Height, DepthFormat, MultiSampleCount, MultiSampleQuality);
            }
        }

        protected override void DisposeOverride(bool disposing)
        {
            if (DepthStencil != null)
            {
                DepthStencil.Dispose();
            }

            base.DisposeOverride(disposing);
        }
    }
}
