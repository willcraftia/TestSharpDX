#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public interface ITexture2D : ITexture
    {
        int Width { get; }

        int Height { get; }

        int MultiSampleCount { get; }

        int MultiSampleQuality { get; }
    }
}
