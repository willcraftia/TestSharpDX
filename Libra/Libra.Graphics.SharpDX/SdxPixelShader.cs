#region Using

using System;

using D3D11PixelShader = SharpDX.Direct3D11.PixelShader;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxPixelShader : IPixelShader
    {
        public D3D11PixelShader D3D11PixelShader { get; private set; }

        public SdxPixelShader(D3D11PixelShader d3d11PixelShader)
        {
            if (d3d11PixelShader == null) throw new ArgumentNullException("d3d11PixelShader");

            D3D11PixelShader = d3d11PixelShader;
        }

        #region IDisposable

        public bool IsDisposed { get; private set; }

        ~SdxPixelShader()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            if (disposing)
            {
                D3D11PixelShader.Dispose();
            }

            IsDisposed = true;
        }

        #endregion
    }
}
