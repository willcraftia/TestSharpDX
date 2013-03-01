#region Using

using System;

using D3D11Buffer = SharpDX.Direct3D11.Buffer;
using D3D11CommonShaderStage = SharpDX.Direct3D11.CommonShaderStage;

#endregion

namespace Libra.Graphics
{
    public abstract class ShaderStage<T> where T : Shader
    {
        #region SamplerStateCollection

        public sealed class SamplerStateCollection
        {
            public const int SlotCount = 16;

            ShaderStage<T> shaderStage;

            SamplerState[] items;

            public SamplerState this[int slot]
            {
                get
                {
                    if ((uint) SlotCount <= (uint) slot) throw new ArgumentOutOfRangeException("slot");

                    return items[slot];
                }
                set
                {
                    if ((uint) SlotCount <= (uint) slot) throw new ArgumentOutOfRangeException("slot");

                    items[slot] = value;

                    if (value != null)
                    {
                        shaderStage.SetSampler(slot, value);
                    }
                }
            }

            internal SamplerStateCollection(ShaderStage<T> shaderStage)
            {
                this.shaderStage = shaderStage;

                items = new SamplerState[SlotCount];
            }
        }

        #endregion

        #region ShaderResourceViewCollection

        public sealed class ShaderResourceViewCollection
        {
            // D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT (128)
            public const int SlotCount = 128;

            ShaderStage<T> shaderStage;

            ShaderResourceView[] items;

            public ShaderResourceView this[int slot]
            {
                get
                {
                    if ((uint) SlotCount <= (uint) slot) throw new ArgumentOutOfRangeException("index");

                    return items[slot];
                }
                set
                {
                    if ((uint) SlotCount <= (uint) slot) throw new ArgumentOutOfRangeException("index");

                    items[slot] = value;

                    if (items[slot] != null)
                    {
                        shaderStage.SetShaderResourceView(slot, value);
                    }
                }
            }

            internal ShaderResourceViewCollection(ShaderStage<T> shaderStage)
            {
                this.shaderStage = shaderStage;

                items = new ShaderResourceView[SlotCount];
            }
        }

        #endregion

        public DeviceContext DeviceContext { get; private set; }

        public abstract T Shader { get; set; }

        public SamplerStateCollection SamplerStates { get; private set; }

        public ShaderResourceViewCollection ShaderResourceViews { get; private set; }

        internal abstract D3D11CommonShaderStage D3D11CommonShaderStage { get; }

        internal ShaderStage(DeviceContext context)
        {
            DeviceContext = context;

            SamplerStates = new SamplerStateCollection(this);
            ShaderResourceViews = new ShaderResourceViewCollection(this);
        }

        public void SetConstantBuffer(int slot, ConstantBuffer buffer)
        {
            D3D11CommonShaderStage.SetConstantBuffer(slot, buffer.D3D11Buffer);
        }

        public void SetConstantBuffers(int startSlot, params ConstantBuffer[] buffers)
        {
            // SharpDX では、定数バッファについて、D3D11 のポインタ渡しコードを隠蔽している。
            // このため、SharpDX のバッファ配列をここで生成するしかない。
            // ただし、一瞬のメソッド終了により破棄対象となりうることにより、
            // GC 世代 #0 に配置されると期待している (PC 環境ならば問題ないはず)。

            var d3d11Buffers = new D3D11Buffer[buffers.Length];
            for (int i = 0; i < buffers.Length; i++)
            {
                d3d11Buffers[i] = buffers[i].D3D11Buffer;
            }

            D3D11CommonShaderStage.SetConstantBuffers(startSlot, d3d11Buffers);
        }

        void SetSampler(int slot, SamplerState sampler)
        {
            D3D11CommonShaderStage.SetSampler(slot, DeviceContext.Device.SamplerStates[sampler]);
        }

        void SetShaderResourceView(int slot, ShaderResourceView shaderResourceView)
        {
            D3D11CommonShaderStage.SetShaderResource(slot, shaderResourceView.D3D11ShaderResourceView);
        }
    }
}
