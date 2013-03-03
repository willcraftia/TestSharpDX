#region Using

using System;

using D3D11Device = SharpDX.Direct3D11.Device;
using D3D11VertexShader = SharpDX.Direct3D11.VertexShader;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxVertexShader : VertexShader
    {
        public D3D11Device D3D11Device { get; private set; }

        public D3D11VertexShader D3D11VertexShader { get; private set; }

        public SdxVertexShader(D3D11Device d3d11Device)
        {
            if (d3d11Device == null) throw new ArgumentNullException("d3d11Device");

            D3D11Device = d3d11Device;
        }

        public override void Initialize(byte[] shaderBytecode)
        {
            if (shaderBytecode == null) throw new ArgumentNullException("shaderBytecode");

            D3D11VertexShader = new D3D11VertexShader(D3D11Device, shaderBytecode);
        }

        #region IDisposable

        protected override void DisposeOverride(bool disposing)
        {
            if (disposing)
            {
                if (D3D11VertexShader != null)
                    D3D11VertexShader.Dispose();
            }

            base.DisposeOverride(disposing);
        }

        #endregion
    }
}
