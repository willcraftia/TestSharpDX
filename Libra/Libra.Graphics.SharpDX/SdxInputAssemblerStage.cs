#region Using

using System;

using D3D11InputAssemblerStage = SharpDX.Direct3D11.InputAssemblerStage;
using D3D11PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology;
using D3D11VertexBufferBinding = SharpDX.Direct3D11.VertexBufferBinding;
using DXGIFormat = SharpDX.DXGI.Format;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxInputAssemblerStage : InputAssemblerStage
    {
        public D3D11InputAssemblerStage D3D11InputAssemblerStage { get; private set; }

        public SdxInputAssemblerStage(D3D11InputAssemblerStage d3d11InputAssemblerStage)
        {
            if (d3d11InputAssemblerStage == null) throw new ArgumentNullException("d3d11InputAssemblerStage");

            D3D11InputAssemblerStage = d3d11InputAssemblerStage;
        }

        protected override void OnInputLayoutChanged()
        {
            if (InputLayout == null)
            {
                D3D11InputAssemblerStage.InputLayout = null;
            }
            else
            {
                D3D11InputAssemblerStage.InputLayout = (InputLayout as SdxInputLayout).D3D11InputLayout;
            }
        }

        protected override void OnPrimitiveTopologyChanged()
        {
            D3D11InputAssemblerStage.PrimitiveTopology = (D3D11PrimitiveTopology) PrimitiveTopology;
        }

        protected override void OnIndexBufferChanged()
        {
            var d3d11Buffer = (IndexBuffer as SdxIndexBuffer).D3D11Buffer;

            D3D11InputAssemblerStage.SetIndexBuffer(d3d11Buffer, (DXGIFormat) IndexBuffer.Format, 0);
        }

        protected override void SetVertexBufferCore(int slot, VertexBufferBinding binding)
        {
            var d3d11VertexBufferBinding = new D3D11VertexBufferBinding
            {
                Buffer = (binding.VertexBuffer as SdxVertexBuffer).D3D11Buffer,
                Offset = binding.Offset,
                Stride = binding.VertexBuffer.VertexDeclaration.Stride
            };

            D3D11InputAssemblerStage.SetVertexBuffers(slot, d3d11VertexBufferBinding);
        }

        protected override void SetVertexBuffersCore(int startSlot, int count, VertexBufferBinding[] bindings)
        {
            // D3D11 のポインタ渡しインタフェースが公開されているため、
            // stackalloc を利用して配列をヒープに作らずに済む方法もあるが、
            // 将来の SharpDX の更新によりインタフェースが隠蔽される可能性もあるため、
            // 配列複製で対応する。

            var d3d11VertexBufferBindings = new D3D11VertexBufferBinding[count];
            for (int i = 0; i < count; i++)
            {
                d3d11VertexBufferBindings[i] = new D3D11VertexBufferBinding
                {
                    Buffer = (bindings[i].VertexBuffer as SdxVertexBuffer).D3D11Buffer,
                    Offset = bindings[i].Offset,
                    Stride = bindings[i].VertexBuffer.VertexDeclaration.Stride
                };
            }

            D3D11InputAssemblerStage.SetVertexBuffers(startSlot, d3d11VertexBufferBindings);
        }
    }
}
