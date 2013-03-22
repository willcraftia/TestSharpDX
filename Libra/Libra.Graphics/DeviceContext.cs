#region Using

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#endregion

namespace Libra.Graphics
{
    public abstract class DeviceContext : IDisposable
    {
        #region MappedSubresource

        internal protected struct MappedSubresource
        {
            public IntPtr Pointer;

            public int RowPitch;

            public int DepthPitch;

            public MappedSubresource(IntPtr pointer, int rowPitch, int depthPitch)
            {
                Pointer = pointer;
                RowPitch = rowPitch;
                DepthPitch = depthPitch;
            }
        }

        #endregion

        #region MapMode

        internal protected enum MapMode
        {
            Read                = 1,
            Write               = 2,
            ReadWrite           = 3,
            WriteDiscard        = 4,
            WriteNoOverwrite    = 5,
        }

        #endregion

        #region ShaderStageState

        sealed class ShaderStageState
        {
            public ConstantBuffer[] ConstantBuffers;

            public SamplerState[] SamplerStates;

            public ShaderResourceView[] ShaderResourceViews;

            public ShaderStageState()
            {
                ConstantBuffers = new ConstantBuffer[ConstantBufferSlotCount];
                SamplerStates = new SamplerState[SamplerSlotCount];
                ShaderResourceViews = new ShaderResourceView[InputResourceSlotCount];
            }
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <remarks>
        /// D3D11.h: D3D11_IA_VERTEX_INPUT_RESOURCE_SLOT_COUNT
        /// </remarks>
        public const int InputResourceSlotCuont = 32;

        /// <summary>
        /// </summary>
        /// <remarks>
        /// D3D11.h:  D3D11_SIMULTANEOUS_RENDER_TARGET_COUNT。
        /// </remarks>
        public const int SimultaneousRenderTargetCount = 8;

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

        InputLayout inputLayout;

        PrimitiveTopology primitiveTopology;

        VertexBufferBinding[] vertexBufferBindings;

        IndexBuffer indexBuffer;

        VertexShader lastVertexShader;

        RasterizerState rasterizerState;

        Viewport viewport;

        Rectangle scissorRectangle;

        BlendState blendState;

        DepthStencilState depthStencilState;

        RenderTargetView[] renderTargetViews;

        ShaderStageState[] shaderStageStates;

        VertexShader vertexShader;

        PixelShader pixelShader;

        public event EventHandler Disposing;

        public IDevice Device { get; private set; }

        public abstract bool Deferred { get; }

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

        public RasterizerState RasterizerState
        {
            get { return rasterizerState; }
            set
            {
                if (rasterizerState == value) return;

                rasterizerState = value;

                OnRasterizerStateChanged();
            }
        }

        public Viewport Viewport
        {
            get { return viewport; }
            set
            {
                viewport = value;

                OnViewportChanged();
            }
        }

        public Rectangle ScissorRectangle
        {
            get { return scissorRectangle; }
            set
            {
                scissorRectangle = value;

                OnScissorRectangleChanged();
            }
        }

        public Color BlendFactor { get; set; }

        public BlendState BlendState
        {
            get { return blendState; }
            set
            {
                if (blendState == value) return;

                blendState = value;

                OnBlendStateChanged();
            }
        }

        public DepthStencilState DepthStencilState
        {
            get { return depthStencilState; }
            set
            {
                if (depthStencilState == value) return;

                depthStencilState = value;

                OnDepthStencilStateChanged();
            }
        }

        public VertexShader VertexShader
        {
            get { return vertexShader; }
            set
            {
                if (vertexShader == value) return;

                vertexShader = value;

                OnVertexShaderChanged();
            }
        }

        public PixelShader PixelShader
        {
            get { return pixelShader; }
            set
            {
                if (pixelShader == value) return;

                pixelShader = value;

                OnPixelShaderChanged();
            }
        }

        protected DeviceContext(IDevice device)
        {
            if (device == null) throw new ArgumentNullException("device");

            Device = device;

            vertexBufferBindings = new VertexBufferBinding[InputResourceSlotCuont];
            AutoResolveInputLayout = true;

            renderTargetViews = new RenderTargetView[SimultaneousRenderTargetCount];

            shaderStageStates = new ShaderStageState[5];
            for (int i = 0; i < shaderStageStates.Length; i++)
            {
                shaderStageStates[i] = new ShaderStageState();
            }
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

        protected abstract void OnInputLayoutChanged();

        protected abstract void OnPrimitiveTopologyChanged();

        protected abstract void OnIndexBufferChanged();

        protected abstract void SetVertexBufferCore(int slot, VertexBufferBinding binding);

        protected abstract void SetVertexBuffersCore(int startSlot, int count, VertexBufferBinding[] bindings);

        protected abstract void OnRasterizerStateChanged();

        protected abstract void OnViewportChanged();

        protected abstract void OnScissorRectangleChanged();

        public RenderTargetView GetRenderTargetView()
        {
            return renderTargetViews[0];
        }

        public void GetRenderTargetViews(RenderTargetView[] result)
        {
            Array.Copy(renderTargetViews, result, Math.Min(SimultaneousRenderTargetCount, result.Length));
        }

        public void SetRenderTargetView(RenderTargetView view)
        {
            renderTargetViews[0] = view;

            SetRenderTargetViewCore(view);
        }

        public void SetRenderTargetViews(params RenderTargetView[] views)
        {
            if (SimultaneousRenderTargetCount < views.Length) throw new ArgumentOutOfRangeException("views");

            Array.Copy(views, renderTargetViews, views.Length);

            SetRenderTargetViewsCore(views);
        }

        protected abstract void SetRenderTargetViewCore(RenderTargetView view);

        protected abstract void SetRenderTargetViewsCore(RenderTargetView[] views);

        protected abstract void OnBlendStateChanged();

        protected abstract void OnDepthStencilStateChanged();

        public ConstantBuffer GetConstantBuffer(ShaderStage shaderStage, int slot)
        {
            if ((uint) ConstantBufferSlotCount <= (uint) slot) throw new ArgumentOutOfRangeException("slot");

            return shaderStageStates[(int) shaderStage].ConstantBuffers[slot];
        }

        public void GetConstantBuffers(ShaderStage shaderStage, int startSlot, int count, ConstantBuffer[] buffers)
        {
            if (buffers == null) throw new ArgumentNullException("buffers");
            if ((uint) ConstantBufferSlotCount <= (uint) startSlot) throw new ArgumentOutOfRangeException("startSlot");
            if ((uint) (ConstantBufferSlotCount - startSlot) < (uint) count ||
                buffers.Length < count) throw new ArgumentOutOfRangeException("count");

            Array.Copy(shaderStageStates[(int) shaderStage].ConstantBuffers, startSlot, buffers, 0, count);
        }

        public void SetConstantBuffer(ShaderStage shaderStage, int slot, ConstantBuffer buffer)
        {
            if ((uint) ConstantBufferSlotCount <= (uint) slot) throw new ArgumentOutOfRangeException("slot");

            shaderStageStates[(int) shaderStage].ConstantBuffers[slot] = buffer;
            SetConstantBufferCore(shaderStage, slot, buffer);
        }

        public void SetConstantBuffers(ShaderStage shaderStage, int startSlot, int count, ConstantBuffer[] buffers)
        {
            if (buffers == null) throw new ArgumentNullException("buffers");
            if ((uint) ConstantBufferSlotCount <= (uint) startSlot) throw new ArgumentOutOfRangeException("startSlot");
            if ((uint) (ConstantBufferSlotCount - startSlot) < (uint) count ||
                buffers.Length < count) throw new ArgumentOutOfRangeException("count");

            Array.Copy(buffers, 0, shaderStageStates[(int) shaderStage].ConstantBuffers, startSlot, count);
            SetConstantBuffersCore(shaderStage, startSlot, count, buffers);
        }

        public SamplerState GetSamplerState(ShaderStage shaderStage, int slot)
        {
            if ((uint) SamplerSlotCount <= (uint) slot) throw new ArgumentOutOfRangeException("slot");

            return shaderStageStates[(int) shaderStage].SamplerStates[slot];
        }

        public void GetSamplerStates(ShaderStage shaderStage, int startSlot, int count, SamplerState[] states)
        {
            if (states == null) throw new ArgumentNullException("states");
            if ((uint) SamplerSlotCount <= (uint) startSlot) throw new ArgumentOutOfRangeException("startSlot");
            if ((uint) (SamplerSlotCount - startSlot) < (uint) count ||
                states.Length < count) throw new ArgumentOutOfRangeException("count");

            Array.Copy(shaderStageStates[(int) shaderStage].SamplerStates, startSlot, states, 0, count);
        }

        public void SetSamplerState(ShaderStage shaderStage, int slot, SamplerState state)
        {
            if ((uint) SamplerSlotCount <= (uint) slot) throw new ArgumentOutOfRangeException("slot");

            shaderStageStates[(int) shaderStage].SamplerStates[slot] = state;
            SetSamplerStateCore(shaderStage, slot, state);
        }

        public void SetSamplerStates(ShaderStage shaderStage, int startSlot, int count, SamplerState[] states)
        {
            if (states == null) throw new ArgumentNullException("states");
            if ((uint) SamplerSlotCount <= (uint) startSlot) throw new ArgumentOutOfRangeException("startSlot");
            if ((uint) (SamplerSlotCount - startSlot) < (uint) count ||
                states.Length < count) throw new ArgumentOutOfRangeException("count");

            Array.Copy(states, 0, shaderStageStates[(int) shaderStage].SamplerStates, startSlot, count);
            SetSamplerStatesCore(shaderStage, startSlot, count, states);
        }

        public ShaderResourceView GetShaderResourceView(ShaderStage shaderStage, int slot)
        {
            if ((uint) InputResourceSlotCount <= (uint) slot) throw new ArgumentOutOfRangeException("slot");

            return shaderStageStates[(int) shaderStage].ShaderResourceViews[slot];
        }

        public void GetShaderResourceViews(ShaderStage shaderStage, int startSlot, int count, ShaderResourceView[] views)
        {
            if (views == null) throw new ArgumentNullException("views");
            if ((uint) InputResourceSlotCount <= (uint) startSlot) throw new ArgumentOutOfRangeException("startSlot");
            if ((uint) (InputResourceSlotCount - startSlot) < (uint) count ||
                views.Length < count) throw new ArgumentOutOfRangeException("count");

            Array.Copy(shaderStageStates[(int) shaderStage].ShaderResourceViews, startSlot, views, 0, count);
        }

        public void SetShaderResourceView(ShaderStage shaderStage, int slot, ShaderResourceView view)
        {
            if ((uint) InputResourceSlotCount <= (uint) slot) throw new ArgumentOutOfRangeException("slot");

            shaderStageStates[(int) shaderStage].ShaderResourceViews[slot] = view;
            SetShaderResourceViewCore(shaderStage, slot, view);
        }

        public void SetShaderResourceViews(ShaderStage shaderStage, int startSlot, int count, ShaderResourceView[] views)
        {
            if (views == null) throw new ArgumentNullException("views");
            if ((uint) InputResourceSlotCount <= (uint) startSlot) throw new ArgumentOutOfRangeException("startSlot");
            if ((uint) (InputResourceSlotCount - startSlot) < (uint) count ||
                views.Length < count) throw new ArgumentOutOfRangeException("count");

            Array.Copy(views, 0, shaderStageStates[(int) shaderStage].ShaderResourceViews, startSlot, count);
            SetShaderResourceViewsCore(shaderStage, startSlot, count, views);
        }

        protected abstract void OnVertexShaderChanged();

        protected abstract void OnPixelShaderChanged();

        protected abstract void SetConstantBufferCore(ShaderStage shaderStage, int slot, ConstantBuffer buffer);

        protected abstract void SetConstantBuffersCore(ShaderStage shaderStage, int startSlot, int count, ConstantBuffer[] buffers);

        protected abstract void SetSamplerStateCore(ShaderStage shaderStage, int slot, SamplerState state);

        protected abstract void SetSamplerStatesCore(ShaderStage shaderStage, int startSlot, int count, SamplerState[] states);

        protected abstract void SetShaderResourceViewCore(ShaderStage shaderStage, int slot, ShaderResourceView view);

        protected abstract void SetShaderResourceViewsCore(ShaderStage shaderStage, int startSlot, int count, ShaderResourceView[] views);

        public void ClearRenderTargetView(
            RenderTargetView view, ClearOptions options, Color color, float depth, byte stencil)
        {
            ClearRenderTargetView(view, options, color.ToVector4(), depth, stencil);
        }

        public abstract void ClearRenderTargetView(
            RenderTargetView view, ClearOptions options, Vector4 color, float depth, byte stencil);

        public void Clear(Color color)
        {
            Clear(color.ToVector4());
        }

        public void Clear(Vector4 color)
        {
            Clear(ClearOptions.Target | ClearOptions.Depth | ClearOptions.Stencil, color);
        }

        public void Clear(ClearOptions options, Color color, float depth = 1f, byte stencil = 0)
        {
            Clear(options, color.ToVector4(), depth, stencil);
        }

        public void Clear(ClearOptions options, Vector4 color, float depth = 1f, byte stencil = 0)
        {
            // アクティブに設定されている全てのレンダ ターゲットをクリア。
            for (int i = 0; i < renderTargetViews.Length; i++)
            {
                var renderTarget = renderTargetViews[i];
                if (renderTarget != null)
                {
                    ClearRenderTargetView(renderTarget, options, color, depth, stencil);
                }
            }
        }

        public void Draw(int vertexCount, int startVertexLocation = 0)
        {
            ApplyState();

            DrawCore(vertexCount, startVertexLocation);
        }

        public void DrawIndexed(int indexCount, int startIndexLocation = 0, int baseVertexLocation = 0)
        {
            ApplyState();

            DrawIndexedCore(indexCount, startIndexLocation, baseVertexLocation);
        }

        protected abstract void DrawCore(int vertexCount, int startVertexLocation);

        protected abstract void DrawIndexedCore(int indexCount, int startIndexLocation, int baseVertexLocation);

        internal protected abstract MappedSubresource Map(Resource resource, int subresource, MapMode mapMode);

        internal protected abstract void Unmap(Resource resource, int subresource);

        internal protected abstract void UpdateSubresource(
            Resource destinationResource, int destinationSubresource, Box? destinationBox,
            IntPtr sourcePointer, int sourceRowPitch, int sourceDepthPitch);

        void ApplyState()
        {
            if (AutoResolveInputLayout)
            {
                // 入力レイアウト自動解決 ON ならば、
                // 頂点シェーダと頂点宣言の組で入力レイアウトを決定して設定。
                // 仮に明示的に入力レイアウトを設定していたとしても、
                // それは上書き設定する。

                //var vertexShader = VertexShaderStage.VertexShader;

                // TODO
                // スロット #0 は確定なのか否か。

                var vertexBuffer = vertexBufferBindings[0].VertexBuffer;
                if (vertexBuffer == null)
                    throw new InvalidOperationException("VertexBuffer is null in slot 0");

                var inputLayout = vertexShader.GetInputLayout(vertexBuffer.VertexDeclaration);
                InputLayout = inputLayout;
            }
        }

        protected virtual void OnDisposing(object sender, EventArgs e)
        {
            if (Disposing != null)
                Disposing(sender, e);
        }

        #region IDisposable

        public bool IsDisposed { get; private set; }

        ~DeviceContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void DisposeOverride(bool disposing) { }

        void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            OnDisposing(this, EventArgs.Empty);

            DisposeOverride(disposing);

            IsDisposed = true;
        }

        #endregion
    }
}
