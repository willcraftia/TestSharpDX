#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public interface IPixelShaderStage : IShaderStage
    {
        PixelShader Shader { get; set; }
    }
}
