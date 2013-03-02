#region Using

using System;

using D3D11VertexShader = SharpDX.Direct3D11.VertexShader;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxVertexShader : IVertexShader
    {
        public D3D11VertexShader D3D11VertexShader { get; private set; }

        public SdxVertexShader(D3D11VertexShader d3d11VertexShader)
        {
            if (d3d11VertexShader == null) throw new ArgumentNullException("d3d11VertexShader");

            D3D11VertexShader = d3d11VertexShader;
        }

        #region IDisposable

        public bool IsDisposed { get; private set; }

        ~SdxVertexShader()
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
                D3D11VertexShader.Dispose();
            }

            IsDisposed = true;
        }

        #endregion
    }
}
