#region Using

using System;

using D3D11VertexShaderStage = SharpDX.Direct3D11.VertexShaderStage;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxVertexShaderStage : VertexShaderStage
    {
        public SdxDevice Device { get; private set; }

        public D3D11VertexShaderStage D3D11VertexShaderStage { get; private set; }

        public SdxVertexShaderStage(SdxDevice device, D3D11VertexShaderStage d3d11VertexShaderStage)
        {
            if (device == null) throw new ArgumentNullException("d3d11Device");
            if (d3d11VertexShaderStage == null) throw new ArgumentNullException("d3d11VertexShaderStage");

            Device = device;
            D3D11VertexShaderStage = d3d11VertexShaderStage;
        }

        protected override void OnVertexShaderChanged()
        {
            if (VertexShader == null)
            {
                D3D11VertexShaderStage.Set(null);
            }
            else
            {
                var d3d11VertexShader = (VertexShader as SdxVertexShader).D3D11VertexShader;
                D3D11VertexShaderStage.Set(d3d11VertexShader);
            }
        }

        protected override void SetConstantBufferCore(int slot, ConstantBuffer buffer)
        {
            D3D11VertexShaderStage.SetConstantBuffer(slot, buffer);
        }

        protected override void SetConstantBuffersCore(int startSlot, int count, ConstantBuffer[] buffers)
        {
            D3D11VertexShaderStage.SetConstantBuffers(startSlot, count, buffers);
        }

        protected override void SetSamplerStateCore(int slot, SamplerState state)
        {
            D3D11VertexShaderStage.SetSamplerState(Device, slot, state);
        }

        protected override void SetSamplerStatesCore(int startSlot, int count, SamplerState[] states)
        {
            D3D11VertexShaderStage.SetSamplerStates(Device, startSlot, count, states);
        }

        protected override void SetShaderResourceViewCore(int slot, ShaderResourceView view)
        {
            D3D11VertexShaderStage.SetShaderResourceViewCore(slot, view);
        }

        protected override void SetShaderResourceViewsCore(int startSlot, int count, ShaderResourceView[] views)
        {
            D3D11VertexShaderStage.SetShaderResourceViewsCore(startSlot, count, views);
        }
    }
}
