#region Using

using System;
using System.Threading;

using D3D11BindFlags = SharpDX.Direct3D11.BindFlags;
using D3D11ResourceUsage = SharpDX.Direct3D11.ResourceUsage;
using D3D11Texture2D = SharpDX.Direct3D11.Texture2D;
using D3D11Texture2DDescription = SharpDX.Direct3D11.Texture2DDescription;
using DXGIFormat = SharpDX.DXGI.Format;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxDepthStencil : SdxResource, IDepthStencil
    {
        public int Width { get; private set; }

        public int Height { get; private set; }

        public DepthFormat Format { get; private set; }

        public int MultiSampleCount { get; private set; }

        public int MultiSampleQuality { get; private set; }

        internal D3D11Texture2D D3D11Texture2D
        {
            get { return D3D11Resource as D3D11Texture2D; }
        }

        public SdxDepthStencil(D3D11Texture2D d3d11Texture2D)
            : base(d3d11Texture2D)
        {
            var d3d11Texture2DDescription = d3d11Texture2D.Description;

            Width = d3d11Texture2DDescription.Width;
            Height = d3d11Texture2DDescription.Height;
            Format = (DepthFormat) d3d11Texture2DDescription.Format;
            MultiSampleCount = d3d11Texture2DDescription.SampleDescription.Count;
            MultiSampleQuality = d3d11Texture2DDescription.SampleDescription.Quality;
        }
    }
}
