#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public interface ISwapChain : IDisposable
    {
        int BackBufferWidth { get; }

        int BackBufferHeight { get; }

        SurfaceFormat BackBufferFormat { get; }

        int BackBufferMultiSampleCount { get; }

        int BackBufferMultiSampleQuality { get; }

        bool Windowed { get; }

        bool AllowModeSwitch { get; }

        DepthFormat DepthStencilFormat { get; }

        int SyncInterval { get; }

        RenderTargetView RenderTargetView { get; }

        void Present();

        void ResizeBuffers(int width, int height);

        void ResizeBuffers(int width, int height, int bufferCount, SurfaceFormat format);

        void ResizeTarget();

        void ResizeTarget(int width, int height);
    }
}
