#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public interface IDevice : IDisposable
    {
        event EventHandler Disposing;

        //readonly DeviceSettings Settings;

        IAdapter Adapter { get; }

        DeviceProfile Profile { get; }

        //DeviceFeatures Features { get; }

        IDeviceContext ImmediateContext { get; }

        IDeviceContext CreateDeferredContext();

        IVertexShader CreateVertexShader(byte[] shaderBytecode);

        IPixelShader CreatePixelShader(byte[] shaderBytecode);

        IInputLayout CreateInputLayout(byte[] shaderBytecode, params InputElement[] elements);

        IInputLayout CreateInputLayout<T>(byte[] shaderBytecode) where T : IInputType;

        IConstantBuffer CreateConstantBuffer(
            int sizeInBytes,
            ResourceUsage usage = ResourceUsage.Default);

        IConstantBuffer CreateConstantBuffer<T>(
            ResourceUsage usage = ResourceUsage.Default) where T : struct;

        IVertexBuffer CreateVertexBuffer(
            int sizeInBytes,
            ResourceUsage usage = ResourceUsage.Default);

        IVertexBuffer CreateVertexBuffer<T>(
            T[] data,
            ResourceUsage usage = ResourceUsage.Immutable) where T : struct;

        ITexture2D CreateTexture2D(
            int width,
            int height,
            bool mipMap = false,
            SurfaceFormat format = SurfaceFormat.Color,
            int multiSampleCount = 1,
            ResourceUsage usage = ResourceUsage.Default);

        IDepthStencil CreateDepthStencil(
            int width,
            int height,
            DepthFormat format = DepthFormat.Depth24Stencil8,
            int multiSampleCount = 1,
            int multiSampleQuality = 0);

        IRenderTarget CreateRenderTarget(
            int width,
            int height,
            bool mipMap = false,
            SurfaceFormat format = SurfaceFormat.Color,
            int multiSampleCount = 1,
            ResourceUsage resourceUsage = ResourceUsage.Default,
            RenderTargetUsage renderTargetUsage = RenderTargetUsage.Discard);

        IRenderTarget CreateRenderTarget(
            int width,
            int height,
            bool mipMap = false,
            SurfaceFormat format = SurfaceFormat.Color,
            DepthFormat depthFormat = DepthFormat.None,
            int multiSampleCount = 1,
            ResourceUsage resourceUsage = ResourceUsage.Default,
            RenderTargetUsage renderTargetUsage = RenderTargetUsage.Discard);

        IShaderResourceView CreateShaderResourceView(ITexture2D texture2D);

        IDepthStencilView CreateDepthStencilView(IDepthStencil depthStencil);

        IRenderTargetView CreateRenderTargetView(IRenderTarget renderTarget);

        int CheckMultiSampleQualityLevels(SurfaceFormat format, int sampleCount);

        int CheckMultiSampleQualityLevels(DepthFormat format, int sampleCount);
    }
}
