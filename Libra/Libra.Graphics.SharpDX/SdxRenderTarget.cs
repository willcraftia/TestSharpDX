#region Using

using System;

using D3D11Texture2D = SharpDX.Direct3D11.Texture2D;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxRenderTarget : SdxTexture2D, IRenderTarget
    {
        public RenderTargetUsage RenderTargetUsage { get; private set; }

        public IDepthStencil DepthStencil { get; private set; }

        public SdxRenderTarget(
            D3D11Texture2D d3d11Texture2D,
            RenderTargetUsage renderTargetUsage = RenderTargetUsage.Discard,
            IDepthStencil depthStencil = null)
            : base(d3d11Texture2D)
        {
            // TODO
            //
            // RenderTargetUsage.Discard の場合、同一設定のテクスチャを共有できる。
            // デバイスでテクスチャの参照を管理し、
            // テクスチャの IUnknown.AddRef/Release で参照カウンタを制御。
            // Release で参照カウンタが 0 になる場合、デバイスでテクスチャを破棄。
            //
            // 同様の処理を深度ステンシルでも行う。

            RenderTargetUsage = renderTargetUsage;
            DepthStencil = depthStencil;
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
