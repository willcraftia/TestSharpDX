#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Libra.Graphics
{
    public abstract class InputLayout : IDisposable
    {
        protected int InputSlotCount = D3D11Constants.IAVertexInputResourceSlotCount;

        bool initialized;

        public IDevice Device { get; private set; }

        public int InputStride { get; private set; }

        protected InputElement[] Elements { get; private set; }

        protected InputLayout(IDevice device)
        {
            if (device == null) throw new ArgumentNullException("device");

            Device = device;
        }

        public void Initialize(Shader shader, params InputElement[] elements)
        {
            AssertNotInitialized();
            if (shader == null) throw new ArgumentNullException("shader");

            Initialize(shader.ShaderBytecode, elements);
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

            Elements = (InputElement[]) elements.Clone();

            InitializeCore(shaderBytecode);

            initialized = true;
        }

        public void Initialize(Shader shader, VertexDeclaration vertexDeclaration, int slot = 0)
        {
            AssertNotInitialized();
            if (shader == null) throw new ArgumentNullException("shader");

            Initialize(shader.ShaderBytecode, vertexDeclaration, slot);
        }

        public void Initialize(byte[] shaderBytecode, VertexDeclaration vertexDeclaration, int slot = 0)
        {
            AssertNotInitialized();
            if ((uint) InputSlotCount < (uint) slot) throw new ArgumentOutOfRangeException("slot");

            InputStride = vertexDeclaration.Stride;

            Elements = vertexDeclaration.GetInputElements(slot);

            InitializeCore(shaderBytecode);

            initialized = true;
        }

        public void Initialize<T>(Shader shader, int slot = 0) where T : IVertexType, new()
        {
            AssertNotInitialized();
            if (shader == null) throw new ArgumentNullException("shader");

            Initialize<T>(shader.ShaderBytecode, slot);
        }

        public void Initialize<T>(byte[] shaderBytecode, int slot = 0) where T : IVertexType, new()
        {
            Initialize(shaderBytecode, new T().VertexDeclaration, slot);
        }

        protected abstract void InitializeCore(byte[] shaderBytecode);

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
