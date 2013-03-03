#region Using

using System;

using D3D11Buffer = SharpDX.Direct3D11.Buffer;
using D3D11CommonShaderStage = SharpDX.Direct3D11.CommonShaderStage;

#endregion

namespace Libra.Graphics.SharpDX
{
    internal abstract class SdxShaderStage<TD3D11ShaderStage> : IShaderStage 
        where TD3D11ShaderStage : D3D11CommonShaderStage
    {
        #region SamplerStateCollection

        public sealed class SamplerStateCollection : ISamplerStateCollection
        {
            public const int SlotCount = 16;

            SdxShaderStage<TD3D11ShaderStage> shaderStage;

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

            internal SamplerStateCollection(SdxShaderStage<TD3D11ShaderStage> shaderStage)
            {
                this.shaderStage = shaderStage;

                items = new SamplerState[SlotCount];
            }
        }

        #endregion

        #region ShaderResourceViewCollection

        public sealed class ShaderResourceViewCollection : IShaderResourceViewCollection
        {
            // D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT (128)
            public const int SlotCount = 128;

            SdxShaderStage<TD3D11ShaderStage> shaderStage;

            SdxShaderResourceView[] items;

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

                    var sdxShaderResourceView = value as SdxShaderResourceView;

                    items[slot] = sdxShaderResourceView;

                    if (items[slot] != null)
                    {
                        shaderStage.SetShaderResourceView(slot, sdxShaderResourceView);
                    }
                }
            }

            internal ShaderResourceViewCollection(SdxShaderStage<TD3D11ShaderStage> shaderStage)
            {
                this.shaderStage = shaderStage;

                items = new SdxShaderResourceView[SlotCount];
            }
        }

        #endregion

        public ISamplerStateCollection SamplerStates { get; private set; }

        public IShaderResourceViewCollection ShaderResourceViews { get; private set; }

        protected SdxDeviceContext DeviceContext { get; private set; }

        protected TD3D11ShaderStage D3D11ShaderStage { get; private set; }

        protected SdxShaderStage(SdxDeviceContext context, TD3D11ShaderStage d3d11ShaderStage)
        {
            DeviceContext = context;
            D3D11ShaderStage = d3d11ShaderStage;

            SamplerStates = new SamplerStateCollection(this);
            ShaderResourceViews = new ShaderResourceViewCollection(this);
        }

        public void SetConstantBuffer(int slot, ConstantBuffer buffer)
        {
            var d3d11Buffer = (buffer as SdxConstantBuffer).D3D11Buffer;
            D3D11ShaderStage.SetConstantBuffer(slot, d3d11Buffer);
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
                d3d11Buffers[i] = (buffers[i] as SdxConstantBuffer).D3D11Buffer;
            }

            D3D11ShaderStage.SetConstantBuffers(startSlot, d3d11Buffers);
        }

        void SetSampler(int slot, SamplerState sampler)
        {
            var device = DeviceContext.Device as SdxDevice;
            D3D11ShaderStage.SetSampler(slot, device.SamplerStateManager[sampler]);
        }

        void SetShaderResourceView(int slot, SdxShaderResourceView shaderResourceView)
        {
            D3D11ShaderStage.SetShaderResource(slot, shaderResourceView.D3D11ShaderResourceView);
        }
    }
}
