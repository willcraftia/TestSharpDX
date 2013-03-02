#region Using

using System;

using D3D11InputLayout = SharpDX.Direct3D11.InputLayout;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxInputLayout : IInputLayout
    {
        public D3D11InputLayout D3D11InputLayout { get; private set; }

        public int InputStride { get; private set; }

        public SdxInputLayout(D3D11InputLayout d3d11InputLayout, int inputStride)
        {
            D3D11InputLayout = d3d11InputLayout;
            InputStride = inputStride;
        }

        #region IDisposable

        bool disposed;

        ~SdxInputLayout()
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
            if (disposed) return;

            if (disposing)
            {
                D3D11InputLayout.Dispose();
            }

            disposed = true;
        }

        #endregion
    }
}
