#region Using

using System;

using D3D11DeviceChild = SharpDX.Direct3D11.DeviceChild;

#endregion

namespace Libra.Graphics
{
    public abstract class Shader : IDisposable
    {
        internal D3D11DeviceChild D3D11DeviceChild { get; private set; }

        internal Shader(Device device, byte[] shaderBytecode)
        {
            D3D11DeviceChild = CreateD3D11DeviceChild(device, shaderBytecode);
        }

        internal abstract D3D11DeviceChild CreateD3D11DeviceChild(Device device, byte[] shaderBytecode);

        #region IDisposable

        public bool IsDisposed { get; private set; }

        ~Shader()
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
                D3D11DeviceChild.Dispose();
            }

            IsDisposed = true;
        }

        #endregion
    }
}
