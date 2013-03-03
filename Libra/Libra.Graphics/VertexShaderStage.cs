#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class VertexShaderStage : ShaderStage
    {
        VertexShader vertexShader;

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

        protected VertexShaderStage() { }

        protected abstract void OnVertexShaderChanged();
    }
}
