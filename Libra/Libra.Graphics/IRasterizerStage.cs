#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public interface IRasterizerStage : IStage
    {
        RasterizerState RasterizerState { get; set; }

        Viewport Viewport { get; set; }

        Rectangle ScissorRectangle { get; set; }
    }
}
