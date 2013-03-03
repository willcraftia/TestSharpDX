﻿#region Using

using System;
using System.Threading;

using D3D11BindFlags = SharpDX.Direct3D11.BindFlags;
using D3D11Device = SharpDX.Direct3D11.Device;
using D3D11ResourceOptionFlags = SharpDX.Direct3D11.ResourceOptionFlags;
using D3D11ResourceUsage = SharpDX.Direct3D11.ResourceUsage;
using D3D11Texture2D = SharpDX.Direct3D11.Texture2D;
using D3D11Texture2DDescription = SharpDX.Direct3D11.Texture2DDescription;
using DXGIFormat = SharpDX.DXGI.Format;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxDepthStencil : DepthStencil
    {
        public D3D11Device D3D11Device { get; private set; }

        public D3D11Texture2D D3D11Texture2D { get; private set; }

        public SdxDepthStencil(D3D11Device d3d11Device)
        {
            if (d3d11Device == null) throw new ArgumentNullException("d3d11Device");

            D3D11Device = d3d11Device;
        }

        public override void Initialize()
        {
            if (Format == DepthFormat.None) throw new InvalidOperationException("Format must be not 'None'.");

            D3D11Texture2DDescription description;
            CreateD3D11Texture2DDescription(out description);

            D3D11Texture2D = new D3D11Texture2D(D3D11Device, description);
        }

        // TODO
        //
        // この抽象メソッドが邪魔なわけだが。

        public override void GetData<T>(IDeviceContext context, int level, T[] data, int startIndex, int elementCount)
        {
            (context as SdxDeviceContext).GetData(this, level, data, startIndex, elementCount);
        }

        public override void SetData<T>(IDeviceContext context, T[] data, int startIndex, int elementCount)
        {
            (context as SdxDeviceContext).SetData(this, data, startIndex, elementCount);
        }

        void CreateD3D11Texture2DDescription(out D3D11Texture2DDescription result)
        {
            result = new D3D11Texture2DDescription
            {
                Width = Width,
                Height = Height,
                // ミップ レベルは 1 で固定。
                MipLevels = 1,
                ArraySize = 1,
                Format = (DXGIFormat) Format,
                SampleDescription =
                {
                    Count = MultisampleCount,
                    Quality = MultisampleQuality
                },
                Usage = (D3D11ResourceUsage) Usage,
                BindFlags = D3D11BindFlags.DepthStencil,
                CpuAccessFlags = ResourceHelper.GetD3D11CpuAccessFlags((D3D11ResourceUsage) Usage),
                // ミップ マップ生成無しで固定。
                OptionFlags = D3D11ResourceOptionFlags.None
            };
        }
    }
}
