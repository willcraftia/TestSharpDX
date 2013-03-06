#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class DeviceContext : IDisposable
    {
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

        public abstract void GetData<T>(Resource resource, int level, T[] data, int startIndex, int elementCount) where T : struct;

        public void SetData<T>(Resource resource, params T[] data) where T : struct
        {
            SetData(resource, data, 0, data.Length);
        }

        public abstract void SetData<T>(Resource resource, T[] data, int startIndex, int elementCount) where T : struct;

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
