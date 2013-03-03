#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public interface IShaderStage
    {
        ISamplerStateCollection SamplerStates { get; }

        IShaderResourceViewCollection ShaderResourceViews { get; }

        void SetConstantBuffer(int slot, ConstantBuffer buffer);

        void SetConstantBuffers(int startSlot, params ConstantBuffer[] buffers);
    }
}
