#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public interface IRenderTargetView : IResourceView
    {
        IDepthStencilView DepthStencilView { get; }
    }
}
