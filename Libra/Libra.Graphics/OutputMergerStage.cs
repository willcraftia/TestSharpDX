#region Using

using System;
using System.Collections.ObjectModel;

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

        internal ReadOnlyCollection<RenderTargetView> RenderTargetViews { get; private set; }

        protected OutputMergerStage()
        {
            renderTargetViews = new RenderTargetView[SimultaneousRenderTargetCount];
            RenderTargetViews = Array.AsReadOnly<RenderTargetView>(renderTargetViews);
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

        protected abstract void SetRenderTargetViewCore(RenderTargetView view);

        protected abstract void SetRenderTargetViewsCore(RenderTargetView[] views);

        protected abstract void OnBlendStateChanged();

        protected abstract void OnDepthStencilStateChanged();
    }
}
