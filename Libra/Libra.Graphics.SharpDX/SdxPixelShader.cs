#region Using

using System;

using D3D11Device = SharpDX.Direct3D11.Device;
using D3D11PixelShader = SharpDX.Direct3D11.PixelShader;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxPixelShader : PixelShader
    {
        public D3D11Device D3D11Device { get; private set; }

        public D3D11PixelShader D3D11PixelShader { get; private set; }

        public SdxPixelShader(D3D11Device d3d11Device)
        {
            if (d3d11Device == null) throw new ArgumentNullException("d3d11Device");

            D3D11Device = d3d11Device;
        }

        public override void Initialize(byte[] shaderBytecode)
        {
            if (shaderBytecode == null) throw new ArgumentNullException("shaderBytecode");

            D3D11PixelShader = new D3D11PixelShader(D3D11Device, shaderBytecode);
        }

        #region IDisposable

        protected override void DisposeOverride(bool disposing)
        {
            if (disposing)
            {
                if (D3D11PixelShader != null)
                    D3D11PixelShader.Dispose();
            }

            base.DisposeOverride(disposing);
        }

        #endregion
    }
}
