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

        public event EventHandler Disposing;

        public abstract IDevice Device { get; }

        public abstract bool Deferred { get; }

        protected abstract InputAssemblerStage InputAssemblerStage { get; }

        public abstract VertexShaderStage VertexShaderStage { get; }

        protected abstract RasterizerStage RasterizerStage { get; }

        public abstract PixelShaderStage PixelShaderStage { get; }

        protected abstract OutputMergerStage OutputMergerStage { get; }

        public bool AutoResolveInputLayout
        {
            get { return InputAssemblerStage.AutoResolveInputLayout; }
            set { InputAssemblerStage.AutoResolveInputLayout = value; }
        }

        public InputLayout InputLayout
        {
            get { return InputAssemblerStage.InputLayout; }
            set { InputAssemblerStage.InputLayout = value; }
        }

        public PrimitiveTopology PrimitiveTopology
        {
            get { return InputAssemblerStage.PrimitiveTopology; }
            set { InputAssemblerStage.PrimitiveTopology = value; }
        }

        public IndexBuffer IndexBuffer
        {
            get { return InputAssemblerStage.IndexBuffer; }
            set { InputAssemblerStage.IndexBuffer = value; }
        }

        public RasterizerState RasterizerState
        {
            get { return RasterizerStage.RasterizerState; }
            set { RasterizerStage.RasterizerState = value; }
        }

        public Viewport Viewport
        {
            get { return RasterizerStage.Viewport; }
            set { RasterizerStage.Viewport = value; }
        }

        public Rectangle ScissorRectangle
        {
            get { return RasterizerStage.ScissorRectangle; }
            set { RasterizerStage.ScissorRectangle = value; }
        }

        public Color BlendFactor
        {
            get { return OutputMergerStage.BlendFactor; }
            set { OutputMergerStage.BlendFactor = value; }
        }

        public BlendState BlendState
        {
            get { return OutputMergerStage.BlendState; }
            set { OutputMergerStage.BlendState = value; }
        }

        public DepthStencilState DepthStencilState
        {
            get { return OutputMergerStage.DepthStencilState; }
            set { OutputMergerStage.DepthStencilState = value; }
        }

        public VertexBufferBinding GetVertexBuffer(int slot)
        {
            return InputAssemblerStage.GetVertexBuffer(slot);
        }

        public void GetVertexBuffers(int startSlot, int count, VertexBufferBinding[] bindings)
        {
            InputAssemblerStage.GetVertexBuffers(startSlot, count, bindings);
        }

        public void SetVertexBuffer(int slot, VertexBuffer buffer, int offset = 0)
        {
            InputAssemblerStage.SetVertexBuffer(slot, buffer, offset);
        }

        public void SetVertexBuffer(int slot, VertexBufferBinding binding)
        {
            InputAssemblerStage.SetVertexBuffer(slot, binding);
        }

        public void SetVertexBuffers(int startSlot, int count, VertexBufferBinding[] bindings)
        {
            InputAssemblerStage.SetVertexBuffers(startSlot, count, bindings);
        }

        public RenderTargetView GetRenderTargetView()
        {
            return OutputMergerStage.GetRenderTargetView();
        }

        public void GetRenderTargetViews(RenderTargetView[] result)
        {
            OutputMergerStage.GetRenderTargetViews(result);
        }

        public void SetRenderTargetView(RenderTargetView view)
        {
            OutputMergerStage.SetRenderTargetView(view);
        }

        public void SetRenderTargetViews(params RenderTargetView[] views)
        {
            OutputMergerStage.SetRenderTargetViews(views);
        }

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
            var renderTargetViews = OutputMergerStage.RenderTargetViews;
            var count = renderTargetViews.Count;

            // アクティブに設定されている全てのレンダ ターゲットをクリア。
            for (int i = 0; i < count; i++)
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
            InputAssemblerStage.ApplyState();

            DrawCore(vertexCount, startVertexLocation);
        }

        public void DrawIndexed(int indexCount, int startIndexLocation = 0, int baseVertexLocation = 0)
        {
            InputAssemblerStage.ApplyState();

            DrawIndexedCore(indexCount, startIndexLocation, baseVertexLocation);
        }

        protected abstract void DrawCore(int vertexCount, int startVertexLocation);

        protected abstract void DrawIndexedCore(int indexCount, int startIndexLocation, int baseVertexLocation);

        internal protected abstract MappedSubresource Map(Resource resource, int subresource, MapMode mapMode);

        internal protected abstract void Unmap(Resource resource, int subresource);

        internal protected abstract void UpdateSubresource(
            Resource destinationResource, int destinationSubresource, Box? destinationBox,
            IntPtr sourcePointer, int sourceRowPitch, int sourceDepthPitch);

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
