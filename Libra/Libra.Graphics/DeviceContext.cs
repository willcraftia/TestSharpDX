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

        #region ShaderStage

        protected enum ShaderStage
        {
            Vertex      = 0,
            Hull        = 1,
            Domain      = 2,
            Geometry    = 3,
            Pixel       = 4
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

        public ConstantBuffer GetVertexShaderConstantBuffer(int slot)
        {
            return GetConstantBuffer(ShaderStage.Vertex, slot);
        }

        public ConstantBuffer GetPixelShaderConstantBuffer(int slot)
        {
            return GetConstantBuffer(ShaderStage.Pixel, slot);
        }

        public void GetVertexShaderConstantBuffers(int startSlot, int count, ConstantBuffer[] buffers)
        {
            GetConstantBuffers(ShaderStage.Vertex, startSlot, count, buffers);
        }

        public void GetPixelShaderConstantBuffers(int startSlot, int count, ConstantBuffer[] buffers)
        {
            GetConstantBuffers(ShaderStage.Pixel, startSlot, count, buffers);
        }

        public void SetVertexShaderConstantBuffer(int slot, ConstantBuffer buffer)
        {
            SetConstantBuffer(ShaderStage.Vertex, slot, buffer);
        }

        public void SetPixelShaderConstantBuffer(int slot, ConstantBuffer buffer)
        {
            SetConstantBuffer(ShaderStage.Pixel, slot, buffer);
        }

        public void SetVertexShaderConstantBuffers(int startSlot, int count, ConstantBuffer[] buffers)
        {
            SetConstantBuffers(ShaderStage.Vertex, startSlot, count, buffers);
        }

        public void SetPixelShaderConstantBuffers(int startSlot, int count, ConstantBuffer[] buffers)
        {
            SetConstantBuffers(ShaderStage.Pixel, startSlot, count, buffers);
        }

        ConstantBuffer GetConstantBuffer(ShaderStage shaderStage, int slot)
        {
            if ((uint) ConstantBufferSlotCount <= (uint) slot) throw new ArgumentOutOfRangeException("slot");

            return shaderStageStates[(int) shaderStage].ConstantBuffers[slot];
        }

        void GetConstantBuffers(ShaderStage shaderStage, int startSlot, int count, ConstantBuffer[] buffers)
        {
            if (buffers == null) throw new ArgumentNullException("buffers");
            if ((uint) ConstantBufferSlotCount <= (uint) startSlot) throw new ArgumentOutOfRangeException("startSlot");
            if ((uint) (ConstantBufferSlotCount - startSlot) < (uint) count ||
                buffers.Length < count) throw new ArgumentOutOfRangeException("count");

            Array.Copy(shaderStageStates[(int) shaderStage].ConstantBuffers, startSlot, buffers, 0, count);
        }

        void SetConstantBuffer(ShaderStage shaderStage, int slot, ConstantBuffer buffer)
        {
            if ((uint) ConstantBufferSlotCount <= (uint) slot) throw new ArgumentOutOfRangeException("slot");

            shaderStageStates[(int) shaderStage].ConstantBuffers[slot] = buffer;
            SetConstantBufferCore(shaderStage, slot, buffer);
        }

        void SetConstantBuffers(ShaderStage shaderStage, int startSlot, int count, ConstantBuffer[] buffers)
        {
            if (buffers == null) throw new ArgumentNullException("buffers");
            if ((uint) ConstantBufferSlotCount <= (uint) startSlot) throw new ArgumentOutOfRangeException("startSlot");
            if ((uint) (ConstantBufferSlotCount - startSlot) < (uint) count ||
                buffers.Length < count) throw new ArgumentOutOfRangeException("count");

            Array.Copy(buffers, 0, shaderStageStates[(int) shaderStage].ConstantBuffers, startSlot, count);
            SetConstantBuffersCore(shaderStage, startSlot, count, buffers);
        }

        public SamplerState GetPixelShaderSampler(int slot)
        {
            return GetSampler(ShaderStage.Pixel, slot);
        }

        public void GetPixelShaderSamplers(int startSlot, int count, SamplerState[] states)
        {
            GetSamplers(ShaderStage.Pixel, startSlot, count, states);
        }

        public void SetPixelShaderSampler(int slot, SamplerState state)
        {
            SetSampler(ShaderStage.Pixel, slot, state);
        }

        public void SetPixelShaderSamplers(int startSlot, int count, SamplerState[] states)
        {
            SetSamplers(ShaderStage.Pixel, startSlot, count, states);
        }

        SamplerState GetSampler(ShaderStage shaderStage, int slot)
        {
            if ((uint) SamplerSlotCount <= (uint) slot) throw new ArgumentOutOfRangeException("slot");

            return shaderStageStates[(int) shaderStage].SamplerStates[slot];
        }

        void GetSamplers(ShaderStage shaderStage, int startSlot, int count, SamplerState[] states)
        {
            if (states == null) throw new ArgumentNullException("states");
            if ((uint) SamplerSlotCount <= (uint) startSlot) throw new ArgumentOutOfRangeException("startSlot");
            if ((uint) (SamplerSlotCount - startSlot) < (uint) count ||
                states.Length < count) throw new ArgumentOutOfRangeException("count");

            Array.Copy(shaderStageStates[(int) shaderStage].SamplerStates, startSlot, states, 0, count);
        }

        void SetSampler(ShaderStage shaderStage, int slot, SamplerState state)
        {
            if ((uint) SamplerSlotCount <= (uint) slot) throw new ArgumentOutOfRangeException("slot");

            shaderStageStates[(int) shaderStage].SamplerStates[slot] = state;
            SetSamplerCore(shaderStage, slot, state);
        }

        void SetSamplers(ShaderStage shaderStage, int startSlot, int count, SamplerState[] states)
        {
            if (states == null) throw new ArgumentNullException("states");
            if ((uint) SamplerSlotCount <= (uint) startSlot) throw new ArgumentOutOfRangeException("startSlot");
            if ((uint) (SamplerSlotCount - startSlot) < (uint) count ||
                states.Length < count) throw new ArgumentOutOfRangeException("count");

            Array.Copy(states, 0, shaderStageStates[(int) shaderStage].SamplerStates, startSlot, count);
            SetSamplersCore(shaderStage, startSlot, count, states);
        }

        public ShaderResourceView GetPixelShaderResource(int slot)
        {
            return GetShaderResource(ShaderStage.Pixel, slot);
        }

        public void GetPixelShaderResource(int startSlot, int count, ShaderResourceView[] views)
        {
            GetShaderResource(ShaderStage.Pixel, startSlot, count, views);
        }

        public void SetPixelShaderResource(int slot, ShaderResourceView view)
        {
            SetShaderResource(ShaderStage.Pixel, slot, view);
        }

        public void SetPixelShaderResource(int startSlot, int count, ShaderResourceView[] views)
        {
            SetShaderResources(ShaderStage.Pixel, startSlot, count, views);
        }

        ShaderResourceView GetShaderResource(ShaderStage shaderStage, int slot)
        {
            if ((uint) InputResourceSlotCount <= (uint) slot) throw new ArgumentOutOfRangeException("slot");

            return shaderStageStates[(int) shaderStage].ShaderResourceViews[slot];
        }

        void GetShaderResource(ShaderStage shaderStage, int startSlot, int count, ShaderResourceView[] views)
        {
            if (views == null) throw new ArgumentNullException("views");
            if ((uint) InputResourceSlotCount <= (uint) startSlot) throw new ArgumentOutOfRangeException("startSlot");
            if ((uint) (InputResourceSlotCount - startSlot) < (uint) count ||
                views.Length < count) throw new ArgumentOutOfRangeException("count");

            Array.Copy(shaderStageStates[(int) shaderStage].ShaderResourceViews, startSlot, views, 0, count);
        }

        void SetShaderResource(ShaderStage shaderStage, int slot, ShaderResourceView view)
        {
            if ((uint) InputResourceSlotCount <= (uint) slot) throw new ArgumentOutOfRangeException("slot");

            shaderStageStates[(int) shaderStage].ShaderResourceViews[slot] = view;
            SetShaderResourceCore(shaderStage, slot, view);
        }

        void SetShaderResources(ShaderStage shaderStage, int startSlot, int count, ShaderResourceView[] views)
        {
            if (views == null) throw new ArgumentNullException("views");
            if ((uint) InputResourceSlotCount <= (uint) startSlot) throw new ArgumentOutOfRangeException("startSlot");
            if ((uint) (InputResourceSlotCount - startSlot) < (uint) count ||
                views.Length < count) throw new ArgumentOutOfRangeException("count");

            Array.Copy(views, 0, shaderStageStates[(int) shaderStage].ShaderResourceViews, startSlot, count);
            SetShaderResourceCore(shaderStage, startSlot, count, views);
        }

        protected abstract void OnVertexShaderChanged();

        protected abstract void OnPixelShaderChanged();

        protected abstract void SetConstantBufferCore(ShaderStage shaderStage, int slot, ConstantBuffer buffer);

        protected abstract void SetConstantBuffersCore(ShaderStage shaderStage, int startSlot, int count, ConstantBuffer[] buffers);

        protected abstract void SetSamplerCore(ShaderStage shaderStage, int slot, SamplerState state);

        protected abstract void SetSamplersCore(ShaderStage shaderStage, int startSlot, int count, SamplerState[] states);

        protected abstract void SetShaderResourceCore(ShaderStage shaderStage, int slot, ShaderResourceView view);

        protected abstract void SetShaderResourceCore(ShaderStage shaderStage, int startSlot, int count, ShaderResourceView[] views);

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
