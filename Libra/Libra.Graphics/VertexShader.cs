#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Libra.Graphics
{
    public abstract class VertexShader : IDisposable
    {
        // VertexShader の破棄と同時に InputLayout も破棄したいため、
        // VertexShader で InputLayout のキャッシュを管理。

        Dictionary<VertexDeclaration, InputLayout> inputLayoutMap;

        public IDevice Device { get; private set; }

        public string Name { get; set; }

        protected internal byte[] ShaderBytecode { get; private set; }

        protected VertexShader(IDevice device)
        {
            if (device == null) throw new ArgumentNullException("device");

            Device = device;

            inputLayoutMap = new Dictionary<VertexDeclaration, InputLayout>();
        }

        public void Initialize(byte[] shaderBytecode)
        {
            if (shaderBytecode == null) throw new ArgumentNullException("shaderBytecode");

            ShaderBytecode = shaderBytecode;

            InitializeCore();
        }

        public InputLayout GetInputLayout(VertexDeclaration vertexDeclaration)
        {
            if (vertexDeclaration == null) throw new ArgumentNullException("vertexDeclaration");

            lock (inputLayoutMap)
            {
                InputLayout inputLayout;
                if (!inputLayoutMap.TryGetValue(vertexDeclaration, out inputLayout))
                {
                    inputLayout = Device.CreateInputLayout();
                    inputLayout.Initialize(ShaderBytecode, vertexDeclaration.Elements);

                    inputLayoutMap[vertexDeclaration] = inputLayout;
                }

                return inputLayout;
            }
        }

        protected abstract void InitializeCore();

        #region IDisposable

        public bool IsDisposed { get; private set; }

        ~VertexShader()
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
