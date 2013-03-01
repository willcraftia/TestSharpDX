#region Using

using System;

using D3D11DepthStencilClearFlags = SharpDX.Direct3D11.DepthStencilClearFlags;
using D3D11DepthStencilView = SharpDX.Direct3D11.DepthStencilView;
using D3D11OutputMergerStage = SharpDX.Direct3D11.OutputMergerStage;
using D3D11RenderTargetView = SharpDX.Direct3D11.RenderTargetView;
using SDXColor4 = SharpDX.Color4;

#endregion

namespace Libra.Graphics
{
    public sealed class OutputMergerStage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// D3D11.h:  D3D11_SIMULTANEOUS_RENDER_TARGET_COUNT
        /// </remarks>
        public const int SimultaneousRenderTargetCount = 8;

        D3D11OutputMergerStage d3d11OutputMergerStage;

        RenderTargetView[] activeRenderTargetViews;

        D3D11RenderTargetView[] activeD3D11RenderTargetViews;

        BlendState blendState;

        DepthStencilState depthStencilState;

        public DeviceContext DeviceContext { get; private set; }

        public Color BlendFactor { get; set; }

        public BlendState BlendState
        {
            get { return blendState; }
            set
            {
                if (blendState == value) return;

                blendState = value;

                if (blendState == null)
                {
                    d3d11OutputMergerStage.SetBlendState(null, BlendFactor.ToSDXColor4(), -1);
                }
                else
                {
                    var d3d11BlendState = DeviceContext.Device.BlendStates[blendState];
                    d3d11OutputMergerStage.SetBlendState(
                        d3d11BlendState, blendState.BlendFactor.ToSDXColor4(), blendState.MultiSampleMask);
                }
            }
        }

        public DepthStencilState DepthStencilState
        {
            get { return depthStencilState; }
            set
            {
                if (depthStencilState == value) return;

                depthStencilState = value;

                if (depthStencilState == null)
                {
                    d3d11OutputMergerStage.SetDepthStencilState(null);
                }
                else
                {
                    var d3d11DepthStancilState = DeviceContext.Device.DepthStencilStates[depthStencilState];

                    d3d11OutputMergerStage.SetDepthStencilState(
                        d3d11DepthStancilState, depthStencilState.ReferenceStencil);
                }
            }
        }

        internal OutputMergerStage(DeviceContext context)
        {
            DeviceContext = context;

            d3d11OutputMergerStage = DeviceContext.D3D11DeviceContext.OutputMerger;

            activeRenderTargetViews = new RenderTargetView[SimultaneousRenderTargetCount];
            activeD3D11RenderTargetViews = new D3D11RenderTargetView[SimultaneousRenderTargetCount];
        }

        public RenderTargetView GetRenderTargetView()
        {
            return activeRenderTargetViews[0];
        }

        public void GetRenderTargetViews(RenderTargetView[] result)
        {
            if (result == null) throw new ArgumentNullException("result");
            if (result.Length == 0 || SimultaneousRenderTargetCount < result.Length)
                throw new ArgumentException("Invalid size of array: " + result.Length, "result");

            for (int i = 0; i < result.Length; i++)
                result[i] = activeRenderTargetViews[i];
        }

        public void SetRenderTargetView(RenderTargetView renderTargetView)
        {
            if (renderTargetView == null)
            {
                // null 指定の場合はレンダ ターゲットおよび深度ステンシルを外す。
                d3d11OutputMergerStage.SetTargets((D3D11DepthStencilView) null, (D3D11RenderTargetView[]) null);
            }
            else
            {
                SetRenderTargetViews(renderTargetView);
            }
        }

        public void SetRenderTargetViews(params RenderTargetView[] renderTargetViews)
        {
            if (renderTargetViews.Length == 0)
                throw new ArgumentException("Invalid size of array: 0", "renderTargetViews");

            if (renderTargetViews[0] == null)
                throw new ArgumentException(string.Format("renderTargetViews[{0}] is null.", 0), "renderTargetViews");

            // 深度ステンシルは先頭のレンダ ターゲットの物を利用。
            var depthStencilView = renderTargetViews[0].DepthStencilView;

            D3D11DepthStencilView d3d11DepthStencilView = null;
            if (depthStencilView != null)
            {
                d3d11DepthStencilView = depthStencilView.D3D11DepthStencilView;
            }

            // TODO
            //
            // MRT の場合に各レンダ ターゲット間の整合性 (サイズ等) を確認すべき。

            // インタフェースの差異の関係上、配列要素の参照をコピーして保持。
            for (int i = 0; i < activeD3D11RenderTargetViews.Length; i++)
            {
                if (i < renderTargetViews.Length)
                {
                    if (renderTargetViews[i] == null)
                        throw new ArgumentException(string.Format("renderTargetViews[{0}] is null.", i), "renderTargetViews");

                    activeD3D11RenderTargetViews[i] = renderTargetViews[i].D3D11RenderTargetView;
                    activeRenderTargetViews[i] = renderTargetViews[i];
                }
                else
                {
                    activeD3D11RenderTargetViews[i] = null;
                    activeRenderTargetViews[i] = null;
                }
            }

            d3d11OutputMergerStage.SetTargets(d3d11DepthStencilView, activeD3D11RenderTargetViews);
        }

        // メモ
        //
        // 本来、レンダ ターゲットと深度ステンシルのクリアはデバイス コンテキストだが、
        // それらの設定とクリアの処理は概ね組となる場合が多いと判断し、
        // 出力マージャでそれら機能を纏めることにした。

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

            for (int i = 0; i < activeRenderTargetViews.Length; i++)
            {
                var renderTarget = activeRenderTargetViews[i];
                if (renderTarget != null)
                {
                    ClearRenderTargetView(renderTarget, options, color, depth, stencil);
                }
            }
        }

        public void ClearRenderTargetView(RenderTargetView renderTargetView,
            ClearOptions options, Color color, float depth, byte stencil)
        {
            ClearRenderTargetView(renderTargetView, options, color.ToVector4(), depth, stencil);
        }

        public void ClearRenderTargetView(RenderTargetView renderTargetView,
            ClearOptions options, Vector4 color, float depth, byte stencil)
        {
            if (renderTargetView == null) throw new ArgumentNullException("renderTarget");

            if ((options & ClearOptions.Target) != 0)
            {
                DeviceContext.D3D11DeviceContext.ClearRenderTargetView(
                    renderTargetView.D3D11RenderTargetView, new SDXColor4(color.X, color.Y, color.Z, color.W));
            }

            var depthStencilView = renderTargetView.DepthStencilView;
            if (depthStencilView == null)
                return;

            D3D11DepthStencilClearFlags flags = 0;

            if ((options & ClearOptions.Depth) != 0)
                flags |= D3D11DepthStencilClearFlags.Depth;

            if ((options & ClearOptions.Stencil) != 0)
                flags |= D3D11DepthStencilClearFlags.Stencil;

            if (flags != 0)
            {
                DeviceContext.D3D11DeviceContext.ClearDepthStencilView(
                    depthStencilView.D3D11DepthStencilView, flags, depth, stencil);
            }
        }
    }
}
