#region Using

using System;

using D3D11CommonShaderStage = SharpDX.Direct3D11.CommonShaderStage;
using D3D11PixelShader = SharpDX.Direct3D11.PixelShader;
using D3D11PixelShaderStage = SharpDX.Direct3D11.PixelShaderStage;

#endregion

namespace Libra.Graphics.SharpDX
{
    internal sealed class SdxPixelShaderStage : SdxShaderStage<D3D11PixelShaderStage>, IPixelShaderStage
    {
        SdxPixelShader shader;

        public IPixelShader Shader
        {
            get { return shader; }
            set
            {
                shader = value as SdxPixelShader;

                if (shader == null)
                {
                    // null 設定でステージを無効化。
                    D3D11ShaderStage.Set(null);
                }
                else
                {
                    D3D11ShaderStage.Set(shader.D3D11PixelShader);
                }
            }
        }

        internal SdxPixelShaderStage(SdxDeviceContext context)
            : base(context, context.D3D11DeviceContext.PixelShader)
        {
        }
    }
}
