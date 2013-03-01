#region Using

using System;

using D3D11DeviceChild = SharpDX.Direct3D11.DeviceChild;
using D3D11PixelShader = SharpDX.Direct3D11.PixelShader;

#endregion

namespace Libra.Graphics
{
    public sealed class PixelShader : Shader
    {
        internal D3D11PixelShader D3D11PixelShader
        {
            get { return D3D11DeviceChild as D3D11PixelShader; }
        }

        public PixelShader(Device device, byte[] shaderBytecode)
            : base(device, shaderBytecode)
        {
        }

        internal override D3D11DeviceChild CreateD3D11DeviceChild(Device device, byte[] shaderBytecode)
        {
            return new D3D11PixelShader(device.D3D11Device, shaderBytecode);
        }
    }
}
