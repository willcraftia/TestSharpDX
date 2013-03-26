#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Libra.Graphics
{
    public abstract class InputLayout : IDisposable
    {
        bool initialized;

        public IDevice Device { get; private set; }

        public int InputStride { get; private set; }

        protected InputLayout(IDevice device)
        {
            if (device == null) throw new ArgumentNullException("device");

            Device = device;
        }

        public void Initialize(byte[] shaderBytecode, params InputElement[] elements)
        {
            AssertNotInitialized();
            if (elements == null) throw new ArgumentNullException("elements");
            if (elements.Length == 0) throw new ArgumentException("elements is empty", "elements");

            foreach (var element in elements)
            {
                InputStride += element.SizeInBytes;
            }

            InitializeCore(shaderBytecode, elements);

            initialized = true;
        }

        public void Initialize(byte[] shaderBytecode, VertexDeclaration vertexDeclaration)
        {
            AssertNotInitialized();

            InputStride = vertexDeclaration.Stride;

            InitializeCore(shaderBytecode, vertexDeclaration.Elements);

            initialized = true;
        }

        public void Initialize<T>(byte[] shaderBytecode) where T : IVertexType, new()
        {
            Initialize(shaderBytecode, new T().VertexDeclaration);
        }

        protected abstract void InitializeCore(byte[] shaderBytecode, InputElement[] inputElements);

        void AssertNotInitialized()
        {
            if (initialized) throw new InvalidOperationException("Already initialized.");
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
