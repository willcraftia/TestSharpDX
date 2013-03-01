#region Using

using System;

using D3D11InputAssemblerStage = SharpDX.Direct3D11.InputAssemblerStage;
using D3D11PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology;
using D3D11VertexBufferBinding = SharpDX.Direct3D11.VertexBufferBinding;

#endregion

namespace Libra.Graphics
{
    public sealed class InputAssemblerStage
    {
        // D3D11.h: D3D11_IA_VERTEX_INPUT_RESOURCE_SLOT_COUNT ( 32 )
        public const int InputResourceSlotCuont = 32;

        D3D11InputAssemblerStage d3d11InputAssemblerStage;

        InputLayout inputLayout;

        public DeviceContext DeviceContext { get; private set; }

        public InputLayout InputLayout
        {
            get { return inputLayout; }
            set
            {
                inputLayout = value;

                d3d11InputAssemblerStage.InputLayout = inputLayout.D3D11InputLayout;
            }
        }

        public PrimitiveTopology PrimitiveTopology
        {
            get { return (PrimitiveTopology) d3d11InputAssemblerStage.PrimitiveTopology; }
            set { d3d11InputAssemblerStage.PrimitiveTopology = (D3D11PrimitiveTopology) value; }
        }

        internal InputAssemblerStage(DeviceContext context)
        {
            DeviceContext = context;

            d3d11InputAssemblerStage = DeviceContext.D3D11DeviceContext.InputAssembler;
        }

        public void SetVertexBuffer<T>(int slot, VertexBuffer buffer, int offset = 0) where T : struct
        {
            SetVertexBuffer(slot, buffer, SdxUtilities.SizeOf<T>(), offset);
        }

        public void SetVertexBuffer(int slot, VertexBuffer buffer, int stride, int offset = 0)
        {
            SetVertexBuffer(slot, new VertexBufferBinding(buffer, stride, offset));
        }

        public void SetVertexBuffer(int slot, VertexBufferBinding binding)
        {
            unsafe
            {
                IntPtr buffer;
                if (binding.VertexBuffer == null)
                {
                    buffer = IntPtr.Zero;
                }
                else
                {
                    buffer = binding.VertexBuffer.D3D11Buffer.NativePointer;
                }
                var stride = binding.Stride;
                var offset = binding.Offset;

                d3d11InputAssemblerStage.SetVertexBuffers(
                    slot, 1, new IntPtr(&buffer), new IntPtr(&stride), new IntPtr(&offset));
            }
        }

        public void SetVertexBuffers(int startSlot, params VertexBufferBinding[] bindings)
        {
            // D3D11 のポインタ渡しインタフェースが公開されているため、
            // stackalloc を利用して配列をヒープに作らずに済んでいるが、
            // 将来の SharpDX の更新によりインタフェースが隠蔽される可能性もあるため、
            // 注意が必要。
            // その場合には、GC 世代 #0 に入る事を期待して一時的なヒープへの配列生成を試みる。

            unsafe
            {
                var length = bindings.Length;
                var buffers = stackalloc IntPtr[length];
                var strides = stackalloc int[length];
                var offsets = stackalloc int[length];

                for (int i = 0; i < length; i++)
                {
                    if (bindings[i].VertexBuffer == null)
                    {
                        buffers[i] = IntPtr.Zero;
                    }
                    else
                    {
                        buffers[i] = bindings[i].VertexBuffer.D3D11Buffer.NativePointer;
                    }

                    strides[i] = bindings[i].Stride;
                    offsets[i] = bindings[i].Offset;
                }

                d3d11InputAssemblerStage.SetVertexBuffers(
                    startSlot, length, new IntPtr(buffers), new IntPtr(strides), new IntPtr(offsets));
            }
        }
    }
}
