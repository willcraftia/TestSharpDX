#region Using

using System;
using System.IO;
using System.Runtime.InteropServices;

using D3D11BindFlags = SharpDX.Direct3D11.BindFlags;
using D3D11CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags;
using D3D11Device = SharpDX.Direct3D11.Device;
using D3D11ImageFileFormat = SharpDX.Direct3D11.ImageFileFormat;
using D3D11MapFlags = SharpDX.Direct3D11.MapFlags;
using D3D11MapMode = SharpDX.Direct3D11.MapMode;
using D3D11Resource = SharpDX.Direct3D11.Resource;
using D3D11ResourceOptionFlags = SharpDX.Direct3D11.ResourceOptionFlags;
using D3D11ResourceUsage = SharpDX.Direct3D11.ResourceUsage;
using D3D11Texture2D = SharpDX.Direct3D11.Texture2D;
using D3D11Texture2DDescription = SharpDX.Direct3D11.Texture2DDescription;
using DXGIFormat = SharpDX.DXGI.Format;

#endregion

namespace Libra.Graphics
{
    public class Texture2D : Texture
    {
        // メモ
        //
        // ・ストリームからの生成では ResourceUsage 決定を D3D11Resource.FromStream に委任
        //      これを指定可能とするには D3DX11_IMAGE_LOAD_INFO の全指定などが必要となり、
        //      画像データの自動認識が行えなくなる。
        //      ここでは、自動認識の利点を優先し、ResourceUsage 指定を破棄する。

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int MultiSampleCount { get; private set; }

        public int MultiSampleQuality { get; private set; }

        public Rectangle Bounds
        {
            get { return new Rectangle(0, 0, Width, Height); }
        }

        internal D3D11Texture2D D3D11Texture2D
        {
            get { return D3D11Resource as D3D11Texture2D; }
        }

        // メモ
        //
        // ResourceUsage は GPU 効率のために必要。
        //

        public Texture2D(Device device, int width, int height,
            bool mipMap = false, SurfaceFormat format = SurfaceFormat.Color, ResourceUsage usage = ResourceUsage.Default)
            : this(device, width, height, mipMap, format, 1, usage, D3D11BindFlags.ShaderResource)
        {
        }

        // メモ
        //
        // D3D11BindFlags は Texture2D と RenderTarget2D の区別に必須。
        //

        internal Texture2D(Device device, int width, int height,
            bool mipMap, SurfaceFormat format, int multiSampleCount, ResourceUsage usage, D3D11BindFlags bindFlags)
            : this(device, CreateD3D11Texture2D(device, width, height, mipMap, format, multiSampleCount, usage, bindFlags))
        {
        }

        internal Texture2D(Device device, D3D11Texture2D d3d11Texture2D)
            : base(device, d3d11Texture2D)
        {
            var d3d11Texture2DDescription = d3d11Texture2D.Description;

            Width = d3d11Texture2DDescription.Width;
            Height = d3d11Texture2DDescription.Height;
            MultiSampleCount = d3d11Texture2DDescription.SampleDescription.Count;
            MultiSampleQuality = d3d11Texture2DDescription.SampleDescription.Quality;
        }

        internal static D3D11Texture2D CreateD3D11Texture2D(Device device, int width, int height,
            bool mipMap, SurfaceFormat format, int multiSampleCount, ResourceUsage usage, D3D11BindFlags bindFlags)
        {
            return CreateD3D11Texture2D(
                device.D3D11Device, width, height,
                mipMap, (DXGIFormat) format, multiSampleCount, (D3D11ResourceUsage) usage, bindFlags);
        }

        internal static D3D11Texture2D CreateD3D11Texture2D(Device device, int width, int height,
            bool mipMap, DepthFormat format, int multiSampleCount, ResourceUsage usage, D3D11BindFlags bindFlags)
        {
            return CreateD3D11Texture2D(
                device.D3D11Device, width, height,
                mipMap, (DXGIFormat) format, multiSampleCount, (D3D11ResourceUsage) usage, bindFlags);
        }

        internal static D3D11Texture2D CreateD3D11Texture2D(D3D11Device d3d11Device, int width, int height,
            bool mipMap, DXGIFormat dxgiFormat, int multiSampleCount, D3D11ResourceUsage d3d11ResuorceUsage, D3D11BindFlags bindFlags)
        {
            // 初期データ無しの構築であるため Immutable は禁止。
            if (d3d11ResuorceUsage == D3D11ResourceUsage.Immutable)
                throw new ArgumentException("Usage must not be immutable.", "d3d11ResuorceUsage");

            var description = new D3D11Texture2DDescription
            {
                Width = width,
                Height = height,
                MipLevels = (mipMap) ? 0 : 1,
                ArraySize = 1,
                Format = dxgiFormat,
                SampleDescription =
                {
                    Count = multiSampleCount,
                    Quality = d3d11Device.CheckMultisampleQualityLevels(dxgiFormat, multiSampleCount)
                },
                Usage = d3d11ResuorceUsage,
                BindFlags = bindFlags,
                CpuAccessFlags = ResolveD3D11CpuAccessFlags(d3d11ResuorceUsage),
                OptionFlags = (mipMap) ? D3D11ResourceOptionFlags.GenerateMipMaps : D3D11ResourceOptionFlags.None
            };

            return new D3D11Texture2D(d3d11Device, description);
        }

        public static Texture2D FromStream(Device device, Stream stream)
        {
            var d3d11Texture2D = D3D11Resource.FromStream<D3D11Texture2D>(device.D3D11Device, stream, (int) stream.Length);
            return new Texture2D(device, d3d11Texture2D);
        }

        public void ToStream(DeviceContext context, Stream stream, ImageFileFormat format)
        {
            D3D11Resource.ToStream(context.D3D11DeviceContext, D3D11Resource, (D3D11ImageFileFormat) format, stream);
        }
    }
}
