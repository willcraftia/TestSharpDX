#region Using

using System;

using D3D11DeviceContext = SharpDX.Direct3D11.DeviceContext;
using D3D11DeviceContextType = SharpDX.Direct3D11.DeviceContextType;

#endregion

namespace Libra.Graphics
{
    public sealed class DeviceContext : IDisposable
    {
        public event EventHandler Disposing;

        public Device Device { get; private set; }

        public bool Deferred { get; private set; }

        public InputAssemblerStage InputAssemblerStage { get; private set; }

        public VertexShaderStage VertexShaderStage { get; private set; }

        public RasterizerStage RasterizerStage { get; private set; }

        public PixelShaderStage PixelShaderStage { get; private set; }

        public OutputMergerStage OutputMergerStage { get; private set; }

        internal D3D11DeviceContext D3D11DeviceContext { get; private set; }

        internal DeviceContext(Device device, D3D11DeviceContext d3d11DeviceContext)
        {
            if (device == null) throw new ArgumentNullException("device");
            if (d3d11DeviceContext == null) throw new ArgumentNullException("d3d11DeviceContext");

            Device = device;
            D3D11DeviceContext = d3d11DeviceContext;

            Deferred = (d3d11DeviceContext.TypeInfo == D3D11DeviceContextType.Deferred);

            // パイプライン ステージの初期化。
            InputAssemblerStage = new InputAssemblerStage(this);
            VertexShaderStage = new VertexShaderStage(this);
            RasterizerStage = new RasterizerStage(this);
            PixelShaderStage = new PixelShaderStage(this);
            OutputMergerStage = new OutputMergerStage(this);
        }

        public void Draw(int vertexCount, int startVertexLocation = 0)
        {
            D3D11DeviceContext.Draw(vertexCount, startVertexLocation);
        }

        #region IDisposable

        public bool IsDisposed { get; private set; }

        ~DeviceContext()
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

            if (Disposing != null)
                Disposing(this, EventArgs.Empty);

            if (disposing)
            {
                D3D11DeviceContext.Dispose();
            }

            IsDisposed = true;
        }

        #endregion
    }
}
