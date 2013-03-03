#region Using

using System;

using D3D11CommonShaderStage = SharpDX.Direct3D11.CommonShaderStage;
using D3D11PixelShader = SharpDX.Direct3D11.PixelShader;
using D3D11PixelShaderStage = SharpDX.Direct3D11.PixelShaderStage;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxPixelShaderStage : PixelShaderStage
    {
        public SdxDevice Device { get; private set; }

        public D3D11PixelShaderStage D3D11PixelShaderStage { get; private set; }

        public SdxPixelShaderStage(SdxDevice device, D3D11PixelShaderStage d3d11PixelShaderStage)
        {
            if (device == null) throw new ArgumentNullException("d3d11Device");
            if (d3d11PixelShaderStage == null) throw new ArgumentNullException("d3d11PixelShaderStage");

            Device = device;
            D3D11PixelShaderStage = d3d11PixelShaderStage;
        }

        protected override void OnPixelShaderChanged()
        {
            if (PixelShader == null)
            {
                D3D11PixelShaderStage.Set(null);
            }
            else
            {
                var d3d11VertexShader = (PixelShader as SdxPixelShader).D3D11PixelShader;
                D3D11PixelShaderStage.Set(d3d11VertexShader);
            }
        }

        protected override void SetConstantBufferCore(int slot, ConstantBuffer buffer)
        {
            D3D11PixelShaderStage.SetConstantBuffer(slot, buffer);
        }

        protected override void SetConstantBuffersCore(int startSlot, int count, ConstantBuffer[] buffers)
        {
            D3D11PixelShaderStage.SetConstantBuffers(startSlot, count, buffers);
        }

        protected override void SetSamplerStateCore(int slot, SamplerState state)
        {
            D3D11PixelShaderStage.SetSamplerState(Device, slot, state);
        }

        protected override void SetSamplerStatesCore(int startSlot, int count, SamplerState[] states)
        {
            D3D11PixelShaderStage.SetSamplerStates(Device, startSlot, count, states);
        }

        protected override void SetShaderResourceViewCore(int slot, ShaderResourceView view)
        {
            D3D11PixelShaderStage.SetShaderResourceViewCore(slot, view);
        }

        protected override void SetShaderResourceViewsCore(int startSlot, int count, ShaderResourceView[] views)
        {
            D3D11PixelShaderStage.SetShaderResourceViewsCore(startSlot, count, views);
        }
    }
}
