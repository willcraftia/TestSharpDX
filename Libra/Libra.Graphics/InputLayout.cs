#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Libra.Graphics
{
    public abstract class InputLayout : IDisposable
    {
        public int InputStride { get; protected set; }

        protected InputLayout() { }

        public abstract void Initialize(byte[] shaderBytecode, IList<InputElement> inputElements);

        public void Initialize<T>(byte[] shaderBytecode) where T : IInputType
        {
            var dummyObject = Activator.CreateInstance(typeof(T)) as IInputType;
            Initialize(shaderBytecode, dummyObject.InputElements);
        }

        #region IDisposable

        public bool IsDisposed { get; private set; }

        ~InputLayout()
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
