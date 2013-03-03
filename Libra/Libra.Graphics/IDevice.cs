#region Using

using System;
using System.Collections.Generic;

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

        VertexShader CreateVertexShader();

        PixelShader CreatePixelShader();

        InputLayout CreateInputLayout();

        ConstantBuffer CreateConstantBuffer();

        VertexBuffer CreateVertexBuffer();

        Texture2D CreateTexture2D();

        DepthStencil CreateDepthStencil();

        RenderTarget CreateRenderTarget();

        ShaderResourceView CreateShaderResourceView();

        DepthStencilView CreateDepthStencilView();

        RenderTargetView CreateRenderTargetView();

        int CheckMultiSampleQualityLevels(SurfaceFormat format, int sampleCount);

        int CheckMultiSampleQualityLevels(DepthFormat format, int sampleCount);
    }
}
