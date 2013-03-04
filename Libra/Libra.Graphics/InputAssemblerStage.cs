#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class InputAssemblerStage
    {
        /// <summary>
        /// </summary>
        /// <remarks>
        /// D3D11.h: D3D11_IA_VERTEX_INPUT_RESOURCE_SLOT_COUNT
        /// </remarks>
        public const int InputResourceSlotCuont = 32;

        InputLayout inputLayout;

        PrimitiveTopology primitiveTopology;

        VertexBufferBinding[] vertexBufferBindings;

        IndexBuffer indexBuffer;

        public InputLayout InputLayout
        {
            get { return inputLayout; }
            set
            {
                if (inputLayout == value) return;

                inputLayout = value;

                OnInputLayoutChanged();
            }
        }

        public PrimitiveTopology PrimitiveTopology
        {
            get { return primitiveTopology; }
            set
            {
                if (primitiveTopology == value) return;

                primitiveTopology = value;

                OnPrimitiveTopologyChanged();
            }
        }

        // offset 指定はひとまず無視する。
        // インデックス配列内のオフセットを指定する事が当面ないため。

        public IndexBuffer IndexBuffer
        {
            get { return indexBuffer; }
            set
            {
                if (indexBuffer == value) return;

                indexBuffer = value;

                OnIndexBufferChanged();
            }
        }

        protected InputAssemblerStage()
        {
            vertexBufferBindings = new VertexBufferBinding[InputResourceSlotCuont];
        }

        public VertexBufferBinding GetVertexBuffer(int slot)
        {
            if ((uint) InputResourceSlotCuont < (uint) slot) throw new ArgumentOutOfRangeException("slot");

            return vertexBufferBindings[slot];
        }

        public void GetVertexBuffers(int startSlot, int count, VertexBufferBinding[] bindings)
        {
            if (bindings == null) throw new ArgumentNullException("bindings");
            if ((uint) InputResourceSlotCuont <= (uint) startSlot) throw new ArgumentOutOfRangeException("startSlot");
            if ((uint) (InputResourceSlotCuont - startSlot) < (uint) count ||
                bindings.Length < count) throw new ArgumentOutOfRangeException("count");

            Array.Copy(vertexBufferBindings, startSlot, bindings, 0, count);
        }

        public void SetVertexBuffer<T>(int slot, VertexBuffer buffer, int offset = 0) where T : struct
        {
            if ((uint) InputResourceSlotCuont < (uint) slot) throw new ArgumentOutOfRangeException("slot");
            if (offset < 0) throw new ArgumentOutOfRangeException("offset");

            SetVertexBufferCore<T>(slot, buffer, offset);
        }

        public void SetVertexBuffer(int slot, VertexBuffer buffer, int stride, int offset = 0)
        {
            if ((uint) InputResourceSlotCuont < (uint) slot) throw new ArgumentOutOfRangeException("slot");
            if (stride < 1) throw new ArgumentOutOfRangeException("stride");
            if (offset < 0) throw new ArgumentOutOfRangeException("offset");

            SetVertexBufferCore(slot, buffer, stride, offset);
        }

        public void SetVertexBuffer(int slot, VertexBufferBinding binding)
        {
            if ((uint) InputResourceSlotCuont < (uint) slot) throw new ArgumentOutOfRangeException("slot");

            SetVertexBufferCore(slot, binding);
        }

        public void SetVertexBuffers(int startSlot, int count, VertexBufferBinding[] bindings)
        {
            if (bindings == null) throw new ArgumentNullException("bindings");
            if ((uint) InputResourceSlotCuont <= (uint) startSlot) throw new ArgumentOutOfRangeException("startSlot");
            if ((uint) (InputResourceSlotCuont - startSlot) < (uint) count ||
                bindings.Length < count) throw new ArgumentOutOfRangeException("count");

            Array.Copy(bindings, 0, vertexBufferBindings, startSlot, count);

            SetVertexBuffersCore(startSlot, count, bindings);
        }

        protected abstract void OnInputLayoutChanged();

        protected abstract void OnPrimitiveTopologyChanged();

        protected abstract void OnIndexBufferChanged();

        protected abstract void SetVertexBufferCore<T>(int slot, VertexBuffer buffer, int offset) where T : struct;

        protected abstract void SetVertexBufferCore(int slot, VertexBuffer buffer, int stride, int offset);

        protected abstract void SetVertexBufferCore(int slot, VertexBufferBinding binding);

        protected abstract void SetVertexBuffersCore(int startSlot, int count, VertexBufferBinding[] bindings);
    }
}
