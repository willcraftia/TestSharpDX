#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public interface IVertexShaderStage : IShaderStage
    {
        IVertexShader Shader { get; set; }
    }
}
