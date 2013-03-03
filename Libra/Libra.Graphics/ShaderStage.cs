#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class ShaderStage
    {
        /// <summary>
        /// </summary>
        /// <remarks>
        /// D3D11.h: D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT。
        /// </remarks>
        public const int ConstantBufferSlotCount = 14;

        /// <summary>
        /// </summary>
        /// <remarks>
        /// D3D11.h: D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT。
        /// </remarks>
        public const int SamplerSlotCount = 16;

        /// <summary>
        /// </summary>
        /// <remarks>
        /// D3D11.h: D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT。
        /// </remarks>
        public const int InputResourceSlotCount = 128;

        ConstantBuffer[] constantBuffers;

        SamplerState[] samplerStates;

        ShaderResourceView[] shaderResourceViews;

        protected ShaderStage()
        {
            constantBuffers = new ConstantBuffer[ConstantBufferSlotCount];
            samplerStates = new SamplerState[SamplerSlotCount];
            shaderResourceViews = new ShaderResourceView[InputResourceSlotCount];
        }

        public ConstantBuffer GetConstantBuffer(int slot)
        {
            if ((uint) ConstantBufferSlotCount <= (uint) slot) throw new ArgumentOutOfRangeException("slot");

            return constantBuffers[slot];
        }

        public void GetConstantBuffers(int startSlot, int count, ConstantBuffer[] buffers)
        {
            if (buffers == null) throw new ArgumentNullException("buffers");
            if ((uint) ConstantBufferSlotCount <= (uint) startSlot) throw new ArgumentOutOfRangeException("startSlot");
            if ((uint) (ConstantBufferSlotCount - startSlot) < (uint) count ||
                buffers.Length < count) throw new ArgumentOutOfRangeException("count");

            Array.Copy(constantBuffers, startSlot, buffers, 0, count);
        }

        public void SetConstantBuffer(int slot, ConstantBuffer buffer)
        {
            if ((uint) ConstantBufferSlotCount <= (uint) slot) throw new ArgumentOutOfRangeException("slot");

            constantBuffers[slot] = buffer;

            SetConstantBufferCore(slot, buffer);
        }

        public void SetConstantBuffers(int startSlot, int count, ConstantBuffer[] buffers)
        {
            if (buffers == null) throw new ArgumentNullException("buffers");
            if ((uint) ConstantBufferSlotCount <= (uint) startSlot) throw new ArgumentOutOfRangeException("startSlot");
            if ((uint) (ConstantBufferSlotCount - startSlot) < (uint) count ||
                samplerStates.Length < count) throw new ArgumentOutOfRangeException("count");

            Array.Copy(buffers, 0, constantBuffers, startSlot, count);

            SetConstantBuffersCore(startSlot, count, buffers);
        }

        public SamplerState GetSamplerState(int slot)
        {
            if ((uint) SamplerSlotCount <= (uint) slot) throw new ArgumentOutOfRangeException("slot");

            return samplerStates[slot];
        }

        public void GetSamplerStates(int startSlot, int count, SamplerState[] states)
        {
            if (states == null) throw new ArgumentNullException("states");
            if ((uint) SamplerSlotCount <= (uint) startSlot) throw new ArgumentOutOfRangeException("startSlot");
            if ((uint) (SamplerSlotCount - startSlot) < (uint) count ||
                states.Length < count) throw new ArgumentOutOfRangeException("count");

            Array.Copy(samplerStates, startSlot, states, 0, count);
        }

        public void SetSamplerState(int slot, SamplerState state)
        {
            if ((uint) SamplerSlotCount <= (uint) slot) throw new ArgumentOutOfRangeException("slot");

            samplerStates[slot] = state;

            SetSamplerStateCore(slot, state);
        }

        public void SetSamplerStates(int startSlot, int count, SamplerState[] states)
        {
            if (states == null) throw new ArgumentNullException("states");
            if ((uint) SamplerSlotCount <= (uint) startSlot) throw new ArgumentOutOfRangeException("startSlot");
            if ((uint) (SamplerSlotCount - startSlot) < (uint) count ||
                states.Length < count) throw new ArgumentOutOfRangeException("count");

            Array.Copy(states, 0, samplerStates, startSlot, count);

            SetSamplerStatesCore(startSlot, count, states);
        }

        public ShaderResourceView GetShaderResourceView(int slot)
        {
            if ((uint) InputResourceSlotCount <= (uint) slot) throw new ArgumentOutOfRangeException("slot");

            return shaderResourceViews[slot];
        }

        public void GetShaderResourceViews(int startSlot, int count, ShaderResourceView[] views)
        {
            if (views == null) throw new ArgumentNullException("views");
            if ((uint) InputResourceSlotCount <= (uint) startSlot) throw new ArgumentOutOfRangeException("startSlot");
            if ((uint) (InputResourceSlotCount - startSlot) < (uint) count ||
                views.Length < count) throw new ArgumentOutOfRangeException("count");

            Array.Copy(shaderResourceViews, startSlot, views, 0, count);
        }

        public void SetShaderResourceView(int slot, ShaderResourceView view)
        {
            if ((uint) InputResourceSlotCount <= (uint) slot) throw new ArgumentOutOfRangeException("slot");

            shaderResourceViews[slot] = view;

            SetShaderResourceViewCore(slot, view);
        }

        public void SetShaderResourceViews(int startSlot, int count, ShaderResourceView[] views)
        {
            if (views == null) throw new ArgumentNullException("views");
            if ((uint) InputResourceSlotCount <= (uint) startSlot) throw new ArgumentOutOfRangeException("startSlot");
            if ((uint) (InputResourceSlotCount - startSlot) < (uint) count ||
                views.Length < count) throw new ArgumentOutOfRangeException("count");

            Array.Copy(views, 0, shaderResourceViews, startSlot, count);

            SetShaderResourceViewsCore(startSlot, count, views);
        }

        protected abstract void SetConstantBufferCore(int slot, ConstantBuffer buffer);

        protected abstract void SetConstantBuffersCore(int startSlot, int count, ConstantBuffer[] buffers);

        protected abstract void SetSamplerStateCore(int slot, SamplerState state);

        protected abstract void SetSamplerStatesCore(int startSlot, int count, SamplerState[] states);

        protected abstract void SetShaderResourceViewCore(int slot, ShaderResourceView view);

        protected abstract void SetShaderResourceViewsCore(int startSlot, int count, ShaderResourceView[] views);
    }
}
