#region Using

using System;

using D3D11CommonShaderStage = SharpDX.Direct3D11.CommonShaderStage;
using D3D11VertexShader = SharpDX.Direct3D11.VertexShader;
using D3D11VertexShaderStage = SharpDX.Direct3D11.VertexShaderStage;

#endregion

namespace Libra.Graphics.SharpDX
{
    internal sealed class SdxVertexShaderStage : SdxShaderStage<D3D11VertexShaderStage>, IVertexShaderStage
    {
        SdxVertexShader shader;

        public VertexShader Shader
        {
            get { return shader; }
            set
            {
                shader = value as SdxVertexShader;

                if (shader == null)
                {
                    // null 設定でステージを無効化。
                    D3D11ShaderStage.Set(null);
                }
                else
                {
                    D3D11ShaderStage.Set(shader.D3D11VertexShader);
                }
            }
        }

        internal SdxVertexShaderStage(SdxDeviceContext context)
            : base(context, context.D3D11DeviceContext.VertexShader)
        {
        }
    }
}
