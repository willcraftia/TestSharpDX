#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class PixelShaderStage : ShaderStage
    {
        PixelShader pixelShader;

        public DeviceContext Context { get; private set; }

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

        protected PixelShaderStage(DeviceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            Context = context;
        }

        protected abstract void OnPixelShaderChanged();
    }
}
