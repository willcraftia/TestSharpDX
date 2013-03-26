#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class PixelShader : IDisposable
    {
        bool initialized;

        public IDevice Device { get; private set; }

        public string Name { get; set; }

        protected PixelShader(IDevice device)
        {
            if (device == null) throw new ArgumentNullException("device");

            Device = device;
        }

        public void Initialize(byte[] shaderBytecode)
        {
            if (initialized) throw new InvalidOperationException("Already initialized.");

            InitializeCore(shaderBytecode);

            initialized = true;
        }

        protected abstract void InitializeCore(byte[] shaderBytecode);

        #region IDisposable

        public bool IsDisposed { get; private set; }

        ~PixelShader()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void DisposeOverride(bool disposing) { }

        void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            DisposeOverride(disposing);

            IsDisposed = true;
        }

        #endregion
    }
}
