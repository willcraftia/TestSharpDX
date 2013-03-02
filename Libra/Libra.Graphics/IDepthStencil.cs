#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public interface IDepthStencil : IResource
    {
        int Width { get; }

        int Height { get; }

        DepthFormat Format { get; }

        int MultiSampleCount { get; }

        int MultiSampleQuality { get; }
    }
}
