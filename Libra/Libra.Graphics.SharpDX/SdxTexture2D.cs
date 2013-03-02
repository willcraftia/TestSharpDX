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

namespace Libra.Graphics.SharpDX
{
    public class SdxTexture2D : SdxTexture, ITexture2D
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

        public D3D11Texture2D D3D11Texture2D
        {
            get { return D3D11Resource as D3D11Texture2D; }
        }

        public SdxTexture2D(D3D11Texture2D d3d11Texture2D)
            : base(d3d11Texture2D)
        {
            var d3d11Texture2DDescription = d3d11Texture2D.Description;

            Width = d3d11Texture2DDescription.Width;
            Height = d3d11Texture2DDescription.Height;
            MultiSampleCount = d3d11Texture2DDescription.SampleDescription.Count;
            MultiSampleQuality = d3d11Texture2DDescription.SampleDescription.Quality;
        }

        public static SdxTexture2D FromStream(SdxDevice device, Stream stream)
        {
            var d3d11Texture2D = D3D11Resource.FromStream<D3D11Texture2D>(device.D3D11Device, stream, (int) stream.Length);
            return new SdxTexture2D(d3d11Texture2D);
        }

        // TODO
        //
        // インタフェースに出す？ヘルパで実装する？

        public void ToStream(SdxDeviceContext context, Stream stream, ImageFileFormat format)
        {
            D3D11Resource.ToStream(context.D3D11DeviceContext, D3D11Resource, (D3D11ImageFileFormat) format, stream);
        }
    }
}
