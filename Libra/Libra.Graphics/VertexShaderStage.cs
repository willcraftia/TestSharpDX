#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class VertexShaderStage : ShaderStage
    {
        VertexShader vertexShader;

        public DeviceContext Context { get; private set; }

        public VertexShader VertexShader
        {
            get { return vertexShader; }
            set
            {
                if (vertexShader == value) return;

                vertexShader = value;

                OnVertexShaderChanged();
            }
        }

        protected VertexShaderStage(DeviceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            Context = context;
        }

        protected abstract void OnVertexShaderChanged();
    }
}
