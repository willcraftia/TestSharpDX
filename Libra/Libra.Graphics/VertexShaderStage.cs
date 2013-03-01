#region Using

using System;

using D3D11CommonShaderStage = SharpDX.Direct3D11.CommonShaderStage;
using D3D11VertexShader = SharpDX.Direct3D11.VertexShader;
using D3D11VertexShaderStage = SharpDX.Direct3D11.VertexShaderStage;

#endregion

namespace Libra.Graphics
{
    public sealed class VertexShaderStage : ShaderStage<VertexShader>
    {
        VertexShader shader;

        public override VertexShader Shader
        {
            get { return shader; }
            set
            {
                shader = value;

                if (shader == null)
                {
                    // null 設定でステージを無効化。
                    D3D11VertexShaderStage.Set(null);
                }
                else
                {
                    D3D11VertexShaderStage.Set(shader.D3D11VertexShader);
                }
            }
        }

        internal override D3D11CommonShaderStage D3D11CommonShaderStage
        {
            get { return DeviceContext.D3D11DeviceContext.VertexShader; }
        }

        internal D3D11VertexShaderStage D3D11VertexShaderStage
        {
            get { return D3D11CommonShaderStage as D3D11VertexShaderStage; }
        }

        internal VertexShaderStage(DeviceContext context)
            : base(context)
        {
        }
    }
}
