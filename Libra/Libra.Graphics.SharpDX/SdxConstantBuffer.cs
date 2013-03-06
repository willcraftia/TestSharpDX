﻿#region Using

using System;
using System.Runtime.InteropServices;

using D3D11BindFlags = SharpDX.Direct3D11.BindFlags;
using D3D11Buffer = SharpDX.Direct3D11.Buffer;
using D3D11BufferDescription = SharpDX.Direct3D11.BufferDescription;
using D3D11CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags;
using D3D11Device = SharpDX.Direct3D11.Device;
using D3D11ResourceOptionFlags = SharpDX.Direct3D11.ResourceOptionFlags;
using D3D11ResourceUsage = SharpDX.Direct3D11.ResourceUsage;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxConstantBuffer : ConstantBuffer
    {
        public D3D11Device D3D11Device { get; private set; }

        public D3D11Buffer D3D11Buffer { get; private set; }

        public SdxConstantBuffer(D3D11Device d3d11Device)
        {
            if (d3d11Device == null) throw new ArgumentNullException("d3d11Device");

            D3D11Device = d3d11Device;
        }

        public override void Initialize()
        {
            if (Usage == ResourceUsage.Immutable)
                throw new InvalidOperationException("Usage must be not immutable.");

            D3D11BufferDescription description;
            CreateD3D11BufferDescription(out description);

            D3D11Buffer = new D3D11Buffer(D3D11Device, description);
        }

        public override void Initialize<T>()
        {
            if (Usage == ResourceUsage.Immutable)
                throw new InvalidOperationException("Usage must be not immutable.");

            ByteWidth = SdxUtilities.SizeOf<T>();

            D3D11BufferDescription description;
            CreateD3D11BufferDescription(out description);

            D3D11Buffer = new D3D11Buffer(D3D11Device, description);
        }

        void CreateD3D11BufferDescription(out D3D11BufferDescription result)
        {
            result = new D3D11BufferDescription
            {
                SizeInBytes = ByteWidth,
                Usage = (D3D11ResourceUsage) Usage,
                BindFlags = D3D11BindFlags.ConstantBuffer,
                CpuAccessFlags = ResourceHelper.GetD3D11CpuAccessFlags((D3D11ResourceUsage) Usage),
                OptionFlags = D3D11ResourceOptionFlags.None,
                StructureByteStride = 0
            };
        }

        #region IDisposable

        protected override void DisposeOverride(bool disposing)
        {
            if (disposing)
            {
                if (D3D11Buffer != null)
                    D3D11Buffer.Dispose();
            }

            base.DisposeOverride(disposing);
        }

        #endregion
    }
}
