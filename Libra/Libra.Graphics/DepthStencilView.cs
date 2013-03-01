#region Using

using System;

using D3D11DepthStencilView = SharpDX.Direct3D11.DepthStencilView;
using D3D11DepthStencilViewDescription = SharpDX.Direct3D11.DepthStencilViewDescription;
using D3D11DepthStencilViewDimension = SharpDX.Direct3D11.DepthStencilViewDimension;
using D3D11DepthStencilViewFlags = SharpDX.Direct3D11.DepthStencilViewFlags;

#endregion

namespace Libra.Graphics
{
    internal sealed class DepthStencilView : ResourceView
    {
        internal D3D11DepthStencilView D3D11DepthStencilView
        {
            get { return D3D11ResourceView as D3D11DepthStencilView; }
        }

        // メモ
        //
        // DepthStencil は RenderTarget の破棄で同時に破棄されるため、
        // DepthStencilView では DepthStencil の破棄を担ってはならない。
        // 故に、resourceResponsibility = false 固定でインスタンス化。

        internal DepthStencilView(DepthStencil depthStencil)
            : base(depthStencil, CreateD3D11DepthStencilView(depthStencil), false)
        {
        }

        static D3D11DepthStencilView CreateD3D11DepthStencilView(DepthStencil depthStencil)
        {
            var d3dDevice = depthStencil.Device.D3D11Device;
            var d3d11Texture2D = depthStencil.D3D11Texture2D;
            var d3d11Texture2DDescription = d3d11Texture2D.Description;

            var description = new D3D11DepthStencilViewDescription
            {
                Format = d3d11Texture2DDescription.Format,
                Flags = D3D11DepthStencilViewFlags.None,
                Texture2D =
                {
                    MipSlice = 0
                }
            };

            if (1 < d3d11Texture2DDescription.SampleDescription.Count)
            {
                description.Dimension = D3D11DepthStencilViewDimension.Texture2DMultisampled;
            }
            else
            {
                description.Dimension = D3D11DepthStencilViewDimension.Texture2D;
            }

            return new D3D11DepthStencilView(d3dDevice, d3d11Texture2D, description);
        }
    }
}
