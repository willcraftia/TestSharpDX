#region Using

using System;

using D3D11CommonShaderStage = SharpDX.Direct3D11.CommonShaderStage;
using D3D11PixelShader = SharpDX.Direct3D11.PixelShader;
using D3D11PixelShaderStage = SharpDX.Direct3D11.PixelShaderStage;

#endregion

namespace Libra.Graphics
{
    public sealed class PixelShaderStage : ShaderStage<PixelShader>
    {
        PixelShader shader;

        public override PixelShader Shader
        {
            get { return shader; }
            set
            {
                shader = value;

                if (shader == null)
                {
                    // null 設定でステージを無効化。
                    D3D11PixelShaderStage.Set(null);
                }
                else
                {
                    D3D11PixelShaderStage.Set(shader.D3D11PixelShader);
                }
            }
        }

        internal override D3D11CommonShaderStage D3D11CommonShaderStage
        {
            get { return DeviceContext.D3D11DeviceContext.PixelShader; }
        }

        internal D3D11PixelShaderStage D3D11PixelShaderStage
        {
            get { return D3D11CommonShaderStage as D3D11PixelShaderStage; }
        }

        internal PixelShaderStage(DeviceContext context)
            : base(context)
        {
        }
    }
}
