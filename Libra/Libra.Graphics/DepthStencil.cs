#region Using

using System;
using System.Threading;

using D3D11BindFlags = SharpDX.Direct3D11.BindFlags;
using D3D11ResourceUsage = SharpDX.Direct3D11.ResourceUsage;
using D3D11Texture2D = SharpDX.Direct3D11.Texture2D;
using D3D11Texture2DDescription = SharpDX.Direct3D11.Texture2DDescription;
using DXGIFormat = SharpDX.DXGI.Format;

#endregion

namespace Libra.Graphics
{
    internal sealed class DepthStencil : Resource
    {
        internal int Width { get; private set; }

        internal int Height { get; private set; }

        internal DepthFormat Format { get; private set; }

        internal int MultiSampleCount { get; private set; }

        internal int MultiSampleQuality { get; private set; }

        internal D3D11Texture2D D3D11Texture2D
        {
            get { return D3D11Resource as D3D11Texture2D; }
        }

        internal DepthStencil(Device device, int width, int height, DepthFormat format, int multiSampleCount, int multiSampleQuality)
            : base(device, CreateD3D11Texture2D(device, width, height, format, multiSampleCount, multiSampleQuality))
        {
            Width = width;
            Height = height;
            Format = format;
            MultiSampleCount = multiSampleCount;
            MultiSampleQuality = multiSampleQuality;

            // TODO
            //
            // RenderTargetUsage.Discard のためのリソース共有の試行。
        }

        static D3D11Texture2D CreateD3D11Texture2D(Device device, int width, int height, DepthFormat format,
            int multiSampleCount, int multiSampleQuality)
        {
            if (width < 1) throw new ArgumentOutOfRangeException("width");
            if (height < 1) throw new ArgumentOutOfRangeException("height");
            if (multiSampleCount < 1) throw new ArgumentOutOfRangeException("multiSampleCount");
            if (multiSampleQuality < 0) throw new ArgumentOutOfRangeException("multiSampleQuality");
            if (format == DepthFormat.None) throw new ArgumentException("Invalid format specified.", "format");

            var description = new D3D11Texture2DDescription
            {
                Width = width,
                Height = height,
                MipLevels = 1,
                ArraySize = 1,
                Format = (DXGIFormat) format,
                SampleDescription =
                {
                    Count = multiSampleCount,
                    Quality = multiSampleQuality
                },
                Usage = D3D11ResourceUsage.Default,
                BindFlags = D3D11BindFlags.DepthStencil
            };

            return new D3D11Texture2D(device.D3D11Device, description);
        }
    }
}
