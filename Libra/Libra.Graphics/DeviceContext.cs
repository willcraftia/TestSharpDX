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

        protected internal enum ShaderStage
        {
            Vertex      = 0,
            Hull        = 1,
            Domain      = 2,
            Geometry    = 3,
            Pixel       = 4
        }

        #endregion

        #region ConstantBufferCollection

        public sealed class ConstantBufferCollection
        {
            /// <summary>
            /// </summary>
            /// <remarks>
            /// D3D11.h: D3D11_COMMONSHADER_CONSTANT_BUFFER_API_SLOT_COUNT ( 14 )
            /// </remarks>
            public int Count = 14;

            DeviceContext context;

            ShaderStage shaderStage;

            ConstantBuffer[] buffers;

            int dirtyFlags;

            public ConstantBuffer this[int index]
            {
                get
                {
                    if ((uint) buffers.Length <= (uint) index) throw new ArgumentOutOfRangeException("index");

                    return buffers[index];
                }
                set
                {
                    if ((uint) buffers.Length <= (uint) index) throw new ArgumentOutOfRangeException("index");

                    if (buffers[index] == value)
                        return;

                    buffers[index] = value;

                    dirtyFlags |= 1 << index;
                }
            }

            internal ConstantBufferCollection(DeviceContext context, ShaderStage shaderStage)
            {
                this.context = context;
                this.shaderStage = shaderStage;

                buffers = new ConstantBuffer[Count];
            }

            internal void Apply()
            {
                if (dirtyFlags == 0)
                    return;

                for (int i = 0; i < buffers.Length; i++)
                {
                    int flag = 1 << i;
                    if ((dirtyFlags & flag) != 0)
                    {
                        context.SetConstantBufferCore(shaderStage, i, buffers[i]);

                        dirtyFlags &= ~flag;
                    }

                    if (dirtyFlags == 0)
                        break;
                }
            }
        }

        #endregion

        #region SamplerStateCollection

        public sealed class SamplerStateCollection
        {
            /// <summary>
            /// </summary>
            /// <remarks>
            /// D3D11.h: D3D11_COMMONSHADER_SAMPLER_SLOT_COUNT ( 16 )
            /// </remarks>
            public int Count = 16;

            DeviceContext context;

            ShaderStage shaderStage;

            SamplerState[] samplers;

            int dirtyFlags;

            public SamplerState this[int index]
            {
                get
                {
                    if ((uint) samplers.Length <= (uint) index) throw new ArgumentOutOfRangeException("index");

                    return samplers[index];
                }
                set
                {
                    if ((uint) samplers.Length <= (uint) index) throw new ArgumentOutOfRangeException("index");

                    if (samplers[index] == value)
                        return;

                    samplers[index] = value;

                    dirtyFlags |= 1 << index;
                }
            }

            internal SamplerStateCollection(DeviceContext context, ShaderStage shaderStage)
            {
                this.context = context;
                this.shaderStage = shaderStage;

                samplers = new SamplerState[Count];
            }

            internal void Apply()
            {
                if (dirtyFlags == 0)
                    return;

                for (int i = 0; i < samplers.Length; i++)
                {
                    int flag = 1 << i;
                    if ((dirtyFlags & flag) != 0)
                    {
                        context.SetSamplerCore(shaderStage, i, samplers[i]);

                        dirtyFlags &= ~flag;
                    }

                    if (dirtyFlags == 0)
                        break;
                }
            }
        }

        #endregion

        #region ShaderResourceCollection

        public sealed class ShaderResourceCollection
        {
            /// <summary>
            /// D3D11 の上限は 128 ですが、ここではサンプラの最大スロット数に合わせて 16 とします。
            /// </summary>
            /// <remarks>
            /// D3D11.h: D3D11_COMMONSHADER_INPUT_RESOURCE_SLOT_COUNT ( 128 )
            /// </remarks>
            public int Count = 16;

            DeviceContext context;

            ShaderStage shaderStage;

            IShaderResourceView[] resources;

            int dirtyFlags;

            public IShaderResourceView this[int index]
            {
                get
                {
                    if ((uint) resources.Length <= (uint) index) throw new ArgumentOutOfRangeException("index");

                    return resources[index];
                }
                set
                {
                    if ((uint) resources.Length <= (uint) index) throw new ArgumentOutOfRangeException("index");

                    if (resources[index] == value)
                        return;

                    resources[index] = value;

                    dirtyFlags |= 1 << index;
                }
            }

            internal ShaderResourceCollection(DeviceContext context, ShaderStage shaderStage)
            {
                this.context = context;
                this.shaderStage = shaderStage;

                resources = new IShaderResourceView[Count];
            }

            internal void Apply()
            {
                if (dirtyFlags == 0)
                    return;

                for (int i = 0; i < resources.Length; i++)
                {
                    int flag = 1 << i;
                    if ((dirtyFlags & flag) != 0)
                    {
                        context.SetShaderResourceCore(shaderStage, i, resources[i]);

                        dirtyFlags &= ~flag;
                    }

                    if (dirtyFlags == 0)
                        break;
                }
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

        RenderTargetView[] renderTargets;

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

        public ConstantBufferCollection VertexShaderConstantBuffers { get; private set; }

        public ConstantBufferCollection PixelShaderConstantBuffers { get; private set; }

        public SamplerStateCollection PixelShaderSamplers { get; private set; }

        public ShaderResourceCollection PixelShaderResources { get; private set; }

        protected DeviceContext(IDevice device)
        {
            if (device == null) throw new ArgumentNullException("device");

            Device = device;

            vertexBufferBindings = new VertexBufferBinding[InputResourceSlotCuont];
            AutoResolveInputLayout = true;

            renderTargets = new RenderTargetView[SimultaneousRenderTargetCount];

            VertexShaderConstantBuffers = new ConstantBufferCollection(this, ShaderStage.Vertex);
            PixelShaderConstantBuffers = new ConstantBufferCollection(this, ShaderStage.Pixel);

            PixelShaderSamplers = new SamplerStateCollection(this, ShaderStage.Pixel);

            PixelShaderResources = new ShaderResourceCollection(this, ShaderStage.Pixel);
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

        public RenderTargetView GetRenderTarget()
        {
            return renderTargets[0];
        }

        public void GetRenderTargets(RenderTargetView[] result)
        {
            Array.Copy(renderTargets, result, Math.Min(SimultaneousRenderTargetCount, result.Length));
        }

        public void SetRenderTarget(RenderTargetView renderTarget)
        {
            renderTargets[0] = renderTarget;

            SetRenderTargetCore(renderTarget);
        }

        public void SetRenderTargets(params RenderTargetView[] renderTargets)
        {
            if (SimultaneousRenderTargetCount < renderTargets.Length)
                throw new ArgumentOutOfRangeException("renderTargets");
            if (renderTargets.Length == 0)
                throw new ArgumentException("renderTargets is empty", "renderTargets");
            if (renderTargets[0] == null)
                throw new ArgumentException(string.Format("renderTargets[{0}] is null.", 0), "renderTargets");

            Array.Copy(renderTargets, renderTargets, renderTargets.Length);

            SetRenderTargetsCore(renderTargets);
        }

        protected abstract void SetRenderTargetCore(RenderTargetView renderTarget);

        protected abstract void SetRenderTargetsCore(RenderTargetView[] renderTargets);

        protected abstract void OnBlendStateChanged();

        protected abstract void OnDepthStencilStateChanged();

        protected abstract void OnVertexShaderChanged();

        protected abstract void OnPixelShaderChanged();

        protected abstract void SetConstantBufferCore(ShaderStage shaderStage, int slot, ConstantBuffer buffer);

        protected abstract void SetSamplerCore(ShaderStage shaderStage, int slot, SamplerState state);

        protected abstract void SetShaderResourceCore(ShaderStage shaderStage, int slot, IShaderResourceView view);

        public void ClearRenderTargetView(
            RenderTargetView renderTarget, ClearOptions options, Color color, float depth, byte stencil)
        {
            ClearRenderTargetView(renderTarget, options, color.ToVector4(), depth, stencil);
        }

        public void ClearRenderTargetView(
            RenderTargetView renderTarget, ClearOptions options, Vector4 color, float depth, byte stencil)
        {
            if (renderTarget == null) throw new ArgumentNullException("renderTarget");

            ClearRenderTargetCore(renderTarget, options, color, depth, stencil);
        }

        protected abstract void ClearRenderTargetCore(
            RenderTargetView renderTarget, ClearOptions options, Vector4 color, float depth, byte stencil);

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
            for (int i = 0; i < renderTargets.Length; i++)
            {
                var renderTarget = renderTargets[i];
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

                // TODO
                // スロット #0 は確定なのか否か。

                var vertexBuffer = vertexBufferBindings[0].VertexBuffer;
                if (vertexBuffer == null)
                    throw new InvalidOperationException("VertexBuffer is null in slot 0");

                var inputLayout = vertexShader.GetInputLayout(vertexBuffer.VertexDeclaration);
                InputLayout = inputLayout;
            }

            // ConstantBuffers
            VertexShaderConstantBuffers.Apply();
            PixelShaderConstantBuffers.Apply();

            // Samplers
            PixelShaderSamplers.Apply();

            // ShaderResources
            PixelShaderResources.Apply();
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
