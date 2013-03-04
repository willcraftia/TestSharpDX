#region Using

using System;
using System.IO;

using D3D11BindFlags = SharpDX.Direct3D11.BindFlags;
using D3D11Device = SharpDX.Direct3D11.Device;
using D3D11ImageFileFormat = SharpDX.Direct3D11.ImageFileFormat;
using D3D11Resource = SharpDX.Direct3D11.Resource;
using D3D11ResourceOptionFlags = SharpDX.Direct3D11.ResourceOptionFlags;
using D3D11ResourceUsage = SharpDX.Direct3D11.ResourceUsage;
using D3D11Texture2D = SharpDX.Direct3D11.Texture2D;
using D3D11Texture2DDescription = SharpDX.Direct3D11.Texture2DDescription;
using DXGIFormat = SharpDX.DXGI.Format;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxRenderTarget : RenderTarget
    {
        public D3D11Device D3D11Device { get; private set; }

        public D3D11Texture2D D3D11Texture2D { get; private set; }

        public SdxRenderTarget(D3D11Device d3d11Device)
        {
            if (d3d11Device == null) throw new ArgumentNullException("d3d11Device");

            D3D11Device = d3d11Device;
        }

        // バック バッファ用の特殊な初期化メソッド。
        public void Initialize(D3D11Texture2D d3d11Texture2D)
        {
            if (d3d11Texture2D == null) throw new ArgumentNullException("d3d11Texture2D");

            D3D11Texture2D = d3d11Texture2D;

            var description = D3D11Texture2D.Description;

            if (!FormatHelper.DXGIFormatsAsSurfaceFormat.Contains(description.Format))
                throw new InvalidOperationException("Format not supported: " + description.Format);

            Width = description.Width;
            Height = description.Height;
            MipLevels = description.MipLevels;
            Format = (SurfaceFormat) description.Format;
            MultisampleCount = description.SampleDescription.Count;
            MultisampleQuality = description.SampleDescription.Quality;
            Usage = (ResourceUsage) description.Usage;
        }

        protected override void InitializeRenderTarget()
        {
            if (Usage != ResourceUsage.Default && Usage != ResourceUsage.Staging)
                throw new ArgumentException("ResourceUsage.Default or Staging required.", "usage");
            if (Width < 1) throw new InvalidOperationException("Width < 1: " + Width);
            if (Height < 1) throw new InvalidOperationException("Height < 1: " + Height);
            if (MipLevels < 0) throw new InvalidOperationException("MipLevels < 0: " + MipLevels);
            if (MultisampleCount < 1) throw new InvalidOperationException("MultisampleCount < 1: " + MultisampleCount);
            if (MultisampleQuality < 0) throw new InvalidOperationException("MultisampleQuality < 0: " + MultisampleQuality);

            D3D11Texture2DDescription description;
            CreateD3D11Texture2DDescription(out description);

            D3D11Texture2D = new D3D11Texture2D(D3D11Device, description);
        }

        protected override void InitializeRenderTarget(Stream stream)
        {
            if (Usage != ResourceUsage.Default && Usage != ResourceUsage.Staging)
                throw new ArgumentException("ResourceUsage.Default or Staging required.", "usage");

            D3D11Texture2D = D3D11Resource.FromStream<D3D11Texture2D>(D3D11Device, stream, (int) stream.Length);

            var description = D3D11Texture2D.Description;

            if (!FormatHelper.DXGIFormatsAsSurfaceFormat.Contains(description.Format))
                throw new InvalidOperationException("Format not supported: " + description.Format);

            Width = description.Width;
            Height = description.Height;
            MipLevels = description.MipLevels;
            Format = (SurfaceFormat) description.Format;
            MultisampleCount = description.SampleDescription.Count;
            MultisampleQuality = description.SampleDescription.Quality;
            Usage = (ResourceUsage) description.Usage;
        }

        protected override DepthStencil InitializeDepthStencil()
        {
            var depthStencil = new SdxDepthStencil(D3D11Device)
            {
                Width = Width,
                Height = Height,
                Format = DepthFormat,
                MultisampleCount = MultisampleCount,
                MultisampleQuality = MultisampleQuality
            };

            depthStencil.Initialize();

            return depthStencil;
        }

        public override void Save(DeviceContext context, Stream stream, ImageFileFormat format = ImageFileFormat.Png)
        {
            var d3d11DeviceContext = (context as SdxDeviceContext).D3D11DeviceContext;

            D3D11Resource.ToStream(d3d11DeviceContext, D3D11Texture2D, (D3D11ImageFileFormat) format, stream);
        }

        void CreateD3D11Texture2DDescription(out D3D11Texture2DDescription result)
        {
            result = new D3D11Texture2DDescription
            {
                Width = Width,
                Height = Height,
                MipLevels = MipLevels,
                ArraySize = 1,
                Format = (DXGIFormat) Format,
                SampleDescription =
                {
                    Count = MultisampleCount,
                    Quality = MultisampleQuality
                },
                Usage = (D3D11ResourceUsage) Usage,
                BindFlags = D3D11BindFlags.ShaderResource | D3D11BindFlags.RenderTarget,
                CpuAccessFlags = ResourceHelper.GetD3D11CpuAccessFlags((D3D11ResourceUsage) Usage),
                OptionFlags = (MipLevels != 1) ? D3D11ResourceOptionFlags.GenerateMipMaps : D3D11ResourceOptionFlags.None
            };
        }

        #region IDisposable

        protected override void DisposeOverride(bool disposing)
        {
            if (disposing)
            {
                if (D3D11Texture2D != null)
                    D3D11Texture2D.Dispose();
            }

            base.DisposeOverride(disposing);
        }

        #endregion
    }
}
