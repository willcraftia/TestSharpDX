#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public interface IInputAssemblerStage
    {
        IInputLayout InputLayout { get; set; }

        PrimitiveTopology PrimitiveTopology { get; set; }

        void SetVertexBuffer<T>(int slot, IVertexBuffer buffer, int offset = 0) where T : struct;

        void SetVertexBuffer(int slot, IVertexBuffer buffer, int stride, int offset = 0);

        void SetVertexBuffer(int slot, VertexBufferBinding binding);

        void SetVertexBuffers(int startSlot, params VertexBufferBinding[] bindings);
    }
}
