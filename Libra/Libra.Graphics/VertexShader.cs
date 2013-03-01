#region Using

using System;

using D3D11DeviceChild = SharpDX.Direct3D11.DeviceChild;
using D3D11VertexShader = SharpDX.Direct3D11.VertexShader;

#endregion

namespace Libra.Graphics
{
    public sealed class VertexShader : Shader
    {
        internal D3D11VertexShader D3D11VertexShader
        {
            get { return D3D11DeviceChild as D3D11VertexShader; }
        }

        public VertexShader(Device device, byte[] shaderBytecode)
            : base(device, shaderBytecode)
        {
        }

        internal override D3D11DeviceChild CreateD3D11DeviceChild(Device device, byte[] shaderBytecode)
        {
            return new D3D11VertexShader(device.D3D11Device, shaderBytecode);
        }
    }
}
