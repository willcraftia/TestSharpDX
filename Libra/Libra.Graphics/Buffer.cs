#region Using

using System;

using D3D11BindFlags = SharpDX.Direct3D11.BindFlags;
using D3D11Buffer = SharpDX.Direct3D11.Buffer;
using D3D11BufferDescription = SharpDX.Direct3D11.BufferDescription;
using D3D11ResourceOptionFlags = SharpDX.Direct3D11.ResourceOptionFlags;
using D3D11ResourceUsage = SharpDX.Direct3D11.ResourceUsage;

#endregion

namespace Libra.Graphics
{
    public class Buffer : Resource
    {
        internal D3D11Buffer D3D11Buffer
        {
            get { return D3D11Resource as D3D11Buffer; }
        }

        public Buffer(Device device, D3D11Buffer d3d11Buffer)
            : base(device, d3d11Buffer)
        {
        }

        internal static void CreateD3D11BufferDescription(D3D11BindFlags d3d11BindFlags,
            int sizeInBytes, ResourceUsage usage, out D3D11BufferDescription result)
        {
            result = new D3D11BufferDescription
            {
                SizeInBytes = sizeInBytes,
                Usage = (D3D11ResourceUsage) usage,
                BindFlags = d3d11BindFlags,
                CpuAccessFlags = ResolveD3D11CpuAccessFlags((D3D11ResourceUsage) usage),
                OptionFlags = D3D11ResourceOptionFlags.None,
                StructureByteStride = 0
            };
        }
    }
}
