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

        void GetRenderTargetViews(IRenderTargetView[] result);

        void SetRenderTargetView(IRenderTargetView renderTargetView);

        void SetRenderTargetViews(params IRenderTargetView[] renderTargetViews);

        void Clear(Color color);

        void Clear(Vector4 color);

        void Clear(ClearOptions options, Color color, float depth = 1f, byte stencil = 0);

        void Clear(ClearOptions options, Vector4 color, float depth = 1f, byte stencil = 0);

        void ClearRenderTargetView(IRenderTargetView renderTargetView,
            ClearOptions options, Color color, float depth, byte stencil);

        void ClearRenderTargetView(IRenderTargetView renderTargetView,
            ClearOptions options, Vector4 color, float depth, byte stencil);
    }
}
