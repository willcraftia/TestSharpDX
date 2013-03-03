#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class PixelShaderStage : ShaderStage
    {
        PixelShader pixelShader;

        public PixelShader PixelShader
        {
            get { return pixelShader; }
            set
            {
                if (pixelShader == value) return;

                pixelShader = value;

                OnPixelShaderChanged();
            }
        }

        protected PixelShaderStage() { }

        protected abstract void OnPixelShaderChanged();
    }
}
