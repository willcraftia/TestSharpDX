#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class DeviceContext : IDisposable
    {
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

        public abstract InputAssemblerStage InputAssemblerStage { get; }

        public abstract VertexShaderStage VertexShaderStage { get; }

        public abstract RasterizerStage RasterizerStage { get; }

        public abstract PixelShaderStage PixelShaderStage { get; }

        public abstract OutputMergerStage OutputMergerStage { get; }

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

        public abstract void Draw(int vertexCount, int startVertexLocation = 0);

        public abstract void DrawIndexed(int indexCount, int startIndexLocation = 0, int baseVertexLocation = 0);

        internal protected abstract void GetData<T>(
            Resource resource, int subresource,
            T[] data, int startIndex, int elementCount) where T : struct;

        internal protected abstract void SetData<T>(
            Resource resource, int subresource,
            T[] data, int startIndex, int elementCount) where T : struct;

        internal protected abstract void SetData<T>(
            Resource resource, int subresource,
            T[] data, int sourceIndex, int elementCount,
            int destinationIndex, SetDataOptions options = SetDataOptions.None) where T : struct;

        // メモ
        //
        // 事前に配列を用意して設定する場合はポインタ不要だが、
        // バッファへ順次書き込むロジックを組む場合にはポインタが必要（必須ではないが便利であり素直）。

        internal protected abstract IntPtr Map(Resource resource, int subresource, MapMode mapMode);

        internal protected abstract void Unmap(Resource resource, int subresource);

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
