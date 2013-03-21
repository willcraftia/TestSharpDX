#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Libra.Graphics
{
    public abstract class InputLayout : IDisposable
    {
        public int InputStride { get; private set; }

        protected InputLayout() { }

        public void Initialize(byte[] shaderBytecode, params InputElement[] elements)
        {
            if (elements == null) throw new ArgumentNullException("elements");
            if (elements.Length == 0) throw new ArgumentException("elements is empty", "elements");

            foreach (var element in elements)
            {
                InputStride += element.SizeInBytes;
            }

            InitializeCore(shaderBytecode, elements);
        }

        public void Initialize(byte[] shaderBytecode, VertexDeclaration vertexDeclaration)
        {
            InputStride = vertexDeclaration.Stride;

            InitializeCore(shaderBytecode, vertexDeclaration.Elements);
        }

        public void Initialize<T>(byte[] shaderBytecode) where T : IVertexType, new()
        {
            Initialize(shaderBytecode, new T().VertexDeclaration);
        }

        protected abstract void InitializeCore(byte[] shaderBytecode, InputElement[] inputElements);

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
