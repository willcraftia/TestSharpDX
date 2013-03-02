#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public interface IPixelShaderStage : IShaderStage
    {
        IPixelShader Shader { get; set; }
    }
}
