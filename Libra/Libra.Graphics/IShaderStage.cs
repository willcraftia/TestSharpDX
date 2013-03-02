#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public interface IShaderStage
    {
        ISamplerStateCollection SamplerStates { get; }

        IShaderResourceViewCollection ShaderResourceViews { get; }

        void SetConstantBuffer(int slot, IConstantBuffer buffer);

        void SetConstantBuffers(int startSlot, params IConstantBuffer[] buffers);
    }
}
