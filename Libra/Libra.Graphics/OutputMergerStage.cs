#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class OutputMergerStage
    {
        /// <summary>
        /// </summary>
        /// <remarks>
        /// D3D11.h:  D3D11_SIMULTANEOUS_RENDER_TARGET_COUNT。
        /// </remarks>
        public const int SimultaneousRenderTargetCount = 8;

        BlendState blendState;

        DepthStencilState depthStencilState;

        RenderTargetView[] renderTargetViews;

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

        protected OutputMergerStage()
        {
            renderTargetViews = new RenderTargetView[SimultaneousRenderTargetCount];
        }

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

        public void Clear(Color color)
        {
            Clear(color.ToVector4());
        }

        public void Clear(Vector4 color)
        {
            Clear(ClearOptions.Target, color, 1, 0);
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

        public void ClearRenderTargetView(
            RenderTargetView view, ClearOptions options, Color color, float depth, byte stencil)
        {
            ClearRenderTargetView(view, options, color.ToVector4(), depth, stencil);
        }

        public abstract void ClearRenderTargetView(
            RenderTargetView view, ClearOptions options, Vector4 color, float depth, byte stencil);
        
        protected abstract void SetRenderTargetViewCore(RenderTargetView view);

        protected abstract void SetRenderTargetViewsCore(RenderTargetView[] views);

        protected abstract void OnBlendStateChanged();

        protected abstract void OnDepthStencilStateChanged();
    }
}
