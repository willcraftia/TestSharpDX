#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public interface IOutputMergerStage
    {
        Color BlendFactor { get; }

        BlendState BlendState { get; set; }

        DepthStencilState DepthStencilState { get; set; }

        void GetRenderTargetViews(RenderTargetView[] result);

        void SetRenderTargetView(RenderTargetView renderTargetView);

        void SetRenderTargetViews(params RenderTargetView[] renderTargetViews);

        void Clear(Color color);

        void Clear(Vector4 color);

        void Clear(ClearOptions options, Color color, float depth = 1f, byte stencil = 0);

        void Clear(ClearOptions options, Vector4 color, float depth = 1f, byte stencil = 0);

        void ClearRenderTargetView(RenderTargetView renderTargetView,
            ClearOptions options, Color color, float depth, byte stencil);

        void ClearRenderTargetView(RenderTargetView renderTargetView,
            ClearOptions options, Vector4 color, float depth, byte stencil);
    }
}
