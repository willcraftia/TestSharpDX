#region Using

using System;
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

        RenderTargetView[] renderTargetViews;

        public event EventHandler Disposing;

        public abstract IDevice Device { get; }

        public abstract bool Deferred { get; }

        public abstract VertexShaderStage VertexShaderStage { get; }

        public abstract PixelShaderStage PixelShaderStage { get; }

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

        protected DeviceContext()
        {
            vertexBufferBindings = new VertexBufferBinding[InputResourceSlotCuont];
            AutoResolveInputLayout = true;

            renderTargetViews = new RenderTargetView[SimultaneousRenderTargetCount];
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

                var vertexShader = VertexShaderStage.VertexShader;

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
