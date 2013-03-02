#region Using

using System;

using D3D11Resource = SharpDX.Direct3D11.Resource;
using D3D11ResourceDimension = SharpDX.Direct3D11.ResourceDimension;
using D3D11Texture1D = SharpDX.Direct3D11.Texture1D;
using D3D11Texture2D = SharpDX.Direct3D11.Texture2D;
using D3D11Texture3D = SharpDX.Direct3D11.Texture3D;
using DXGIFormat = SharpDX.DXGI.Format;

#endregion

namespace Libra.Graphics.SharpDX
{
    public abstract class SdxTexture : SdxResource, ITexture
    {
        public SurfaceFormat Format { get; internal set; }

        internal SdxTexture(D3D11Resource d3d11Resource)
            : base(d3d11Resource)
        {
            if (D3D11Resource.Dimension == D3D11ResourceDimension.Buffer)
                throw new ArgumentException("Buffer resource not supported.", "d3d11Resource");

            DXGIFormat dxgiFormat = DXGIFormat.Unknown;
            switch (D3D11Resource.Dimension)
            {
                case D3D11ResourceDimension.Texture1D:
                    dxgiFormat = (D3D11Resource as D3D11Texture1D).Description.Format;
                    break;
                case D3D11ResourceDimension.Texture2D:
                    dxgiFormat = (D3D11Resource as D3D11Texture2D).Description.Format;
                    break;
                case D3D11ResourceDimension.Texture3D:
                    dxgiFormat = (D3D11Resource as D3D11Texture3D).Description.Format;
                    break;
            }

            if (dxgiFormat == DXGIFormat.Unknown)
                throw new InvalidOperationException("Format unknown:");

            Format = (SurfaceFormat) dxgiFormat;
        }
    }
}
