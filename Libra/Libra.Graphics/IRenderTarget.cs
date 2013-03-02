#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public interface IRenderTarget : ITexture2D
    {
        RenderTargetUsage RenderTargetUsage { get; }

        IDepthStencil DepthStencil { get; }
    }
}
