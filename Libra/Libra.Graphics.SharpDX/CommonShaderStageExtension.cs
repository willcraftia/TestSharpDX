#region Using

using System;

using D3D11Buffer = SharpDX.Direct3D11.Buffer;
using D3D11CommonShaderStage = SharpDX.Direct3D11.CommonShaderStage;
using D3D11SamplerState = SharpDX.Direct3D11.SamplerState;
using D3D11ShaderResourceView = SharpDX.Direct3D11.ShaderResourceView;

#endregion

namespace Libra.Graphics.SharpDX
{
    public static class CommonShaderStageExtension
    {
        public static void SetConstantBuffer(this D3D11CommonShaderStage stage,
            int slot, ConstantBuffer buffer)
        {
            if (buffer == null)
            {
                stage.SetConstantBuffer(slot, null);
            }
            else
            {
                var d3d11Buffer = (buffer as SdxConstantBuffer).D3D11Buffer;
                stage.SetConstantBuffer(slot, d3d11Buffer);
            }
        }

        public static void SetConstantBuffers(this D3D11CommonShaderStage stage,
            int startSlot, int count, ConstantBuffer[] buffers)
        {
            var d3d11Buffers = new D3D11Buffer[count];
            for (int i = 0; i < count; i++)
            {
                if (buffers[i] != null)
                    d3d11Buffers[i] = (buffers[i] as SdxConstantBuffer).D3D11Buffer;
            }

            stage.SetConstantBuffers(startSlot, count, d3d11Buffers);
        }

        public static void SetSamplerState(this D3D11CommonShaderStage stage,
            SdxDevice device, int slot, SamplerState state)
        {
            if (state == null)
            {
                stage.SetSampler(slot, null);
            }
            else
            {
                stage.SetSampler(slot, device.SamplerStateManager[state]);
            }
        }

        public static void SetSamplerStates(this D3D11CommonShaderStage stage,
            SdxDevice device, int startSlot, int count, SamplerState[] states)
        {
            var d3d11SamplerStates = new D3D11SamplerState[count];
            for (int i = 0; i < count; i++)
            {
                if (states[i] != null)
                    d3d11SamplerStates[i] = device.SamplerStateManager[states[i]];
            }

            stage.SetSamplers(startSlot, count, d3d11SamplerStates);
        }

        public static void SetShaderResourceViewCore(this D3D11CommonShaderStage stage,
            int slot, ShaderResourceView view)
        {
            if (view == null)
            {
                stage.SetShaderResource(slot, null);
            }
            else
            {
                var d3d11ShaderResourceView = (view as SdxShaderResourceView).D3D11ShaderResourceView;
                stage.SetShaderResource(slot, d3d11ShaderResourceView);
            }
        }

        public static void SetShaderResourceViewsCore(this D3D11CommonShaderStage stage,
            int startSlot, int count, ShaderResourceView[] views)
        {
            var d3d11ShaderResourceViews = new D3D11ShaderResourceView[count];
            for (int i = 0; i < count; i++)
            {
                if (views[i] != null)
                    d3d11ShaderResourceViews[i] = (views[i] as SdxShaderResourceView).D3D11ShaderResourceView;
            }

            stage.SetShaderResources(startSlot, count, d3d11ShaderResourceViews);
        }
    }
}
