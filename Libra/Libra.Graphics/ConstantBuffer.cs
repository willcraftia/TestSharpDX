#region Using

using System;

using D3D11BindFlags = SharpDX.Direct3D11.BindFlags;
using D3D11Buffer = SharpDX.Direct3D11.Buffer;
using D3D11BufferDescription = SharpDX.Direct3D11.BufferDescription;

#endregion

namespace Libra.Graphics
{
    public sealed class ConstantBuffer : Buffer
    {
        public ConstantBuffer(Device device, int sizeInBytes, ResourceUsage usage = ResourceUsage.Default)
            : base(device, CreateD3D11Buffer(device, sizeInBytes, usage))
        {
        }

        public static ConstantBuffer Create<T>(Device device, ResourceUsage usage = ResourceUsage.Default)
            where T : struct
        {
            return new ConstantBuffer(device, SdxUtilities.SizeOf<T>(), usage);
        }

        static D3D11Buffer CreateD3D11Buffer(Device device, int sizeInBytes, ResourceUsage usage)
        {
            D3D11BufferDescription description;
            CreateD3D11BufferDescription(D3D11BindFlags.ConstantBuffer, sizeInBytes, usage, out description);

            return new D3D11Buffer(device.D3D11Device, description);
        }
    }
}
