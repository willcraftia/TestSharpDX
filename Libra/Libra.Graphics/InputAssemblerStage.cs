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

        DeviceContext context;

        InputLayout inputLayout;

        PrimitiveTopology primitiveTopology;

        VertexBufferBinding[] vertexBufferBindings;

        IndexBuffer indexBuffer;

        VertexShader lastVertexShader;

        public bool AutoResolveInputLayout { get; set; }

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

        protected InputAssemblerStage(DeviceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            this.context = context;

            vertexBufferBindings = new VertexBufferBinding[InputResourceSlotCuont];

            AutoResolveInputLayout = true;
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

        public void SetVertexBuffer(int slot, VertexBuffer buffer, int offset = 0)
        {
            if ((uint) InputResourceSlotCuont < (uint) slot) throw new ArgumentOutOfRangeException("slot");
            if (offset < 0) throw new ArgumentOutOfRangeException("offset");

            var binding = new VertexBufferBinding(buffer, offset);
            vertexBufferBindings[slot] = binding;

            SetVertexBufferCore(slot, binding);
        }

        public void SetVertexBuffer(int slot, VertexBufferBinding binding)
        {
            if ((uint) InputResourceSlotCuont < (uint) slot) throw new ArgumentOutOfRangeException("slot");

            vertexBufferBindings[slot] = binding;

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

        internal void ApplyState()
        {
            if (AutoResolveInputLayout)
            {
                // 入力レイアウト自動解決 ON ならば、
                // 頂点シェーダと頂点宣言の組で入力レイアウトを決定して設定。
                // 仮に明示的に入力レイアウトを設定していたとしても、
                // それは上書き設定する。

                var vertexShader = context.VertexShaderStage.VertexShader;

                // TODO
                // スロット #0 は確定なのか否か。

                var vertexBuffer = vertexBufferBindings[0].VertexBuffer;
                if (vertexBuffer == null)
                    throw new InvalidOperationException("VertexBuffer is null in slot 0");

                var inputLayout = vertexShader.GetInputLayout(vertexBuffer.VertexDeclaration);
                InputLayout = inputLayout;
            }
        }

        protected abstract void OnInputLayoutChanged();

        protected abstract void OnPrimitiveTopologyChanged();

        protected abstract void OnIndexBufferChanged();

        protected abstract void SetVertexBufferCore(int slot, VertexBufferBinding binding);

        protected abstract void SetVertexBuffersCore(int startSlot, int count, VertexBufferBinding[] bindings);
    }
}
