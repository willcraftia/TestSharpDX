﻿#region Using

using System;

using D3D11DepthStencilClearFlags = SharpDX.Direct3D11.DepthStencilClearFlags;
using D3D11DepthStencilView = SharpDX.Direct3D11.DepthStencilView;
using D3D11OutputMergerStage = SharpDX.Direct3D11.OutputMergerStage;
using D3D11RenderTargetView = SharpDX.Direct3D11.RenderTargetView;
using SDXColor4 = SharpDX.Color4;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxOutputMergerStage : OutputMergerStage
    {
        D3D11RenderTargetView[] activeD3D11RenderTargetViews;

        public D3D11OutputMergerStage D3D11OutputMergerStage { get; private set; }

        public SdxDevice Device { get; private set; }

        protected override void OnBlendStateChanged()
        {
            if (BlendState == null)
            {
                D3D11OutputMergerStage.SetBlendState(null, BlendFactor.ToSDXColor4(), -1);
            }
            else
            {
                var d3d11BlendState = Device.BlendStateManager[BlendState];
                D3D11OutputMergerStage.SetBlendState(
                    d3d11BlendState, BlendState.BlendFactor.ToSDXColor4(), BlendState.MultiSampleMask);
            }
        }

        protected override void OnDepthStencilStateChanged()
        {
            if (DepthStencilState == null)
            {
                D3D11OutputMergerStage.SetDepthStencilState(null);
            }
            else
            {
                var d3d11DepthStancilState = Device.DepthStencilStateManager[DepthStencilState];

                D3D11OutputMergerStage.SetDepthStencilState(
                    d3d11DepthStancilState, DepthStencilState.ReferenceStencil);
            }
        }

        public SdxOutputMergerStage(SdxDeviceContext context)
            : base(context)
        {
            Device = context.Device as SdxDevice;
            D3D11OutputMergerStage = context.D3D11DeviceContext.OutputMerger;

            activeD3D11RenderTargetViews = new D3D11RenderTargetView[SimultaneousRenderTargetCount];
        }

        protected override void SetRenderTargetViewCore(RenderTargetView view)
        {
            if (view == null)
            {
                // null 指定の場合はレンダ ターゲットおよび深度ステンシルを外す。
                D3D11OutputMergerStage.SetTargets((D3D11DepthStencilView) null, (D3D11RenderTargetView[]) null);
            }
            else
            {

                // 深度ステンシルは先頭のレンダ ターゲットの物を利用。
                var depthStencilView = view.DepthStencilView;

                D3D11DepthStencilView d3d11DepthStencilView = null;
                if (depthStencilView != null)
                {
                    d3d11DepthStencilView = (depthStencilView as SdxDepthStencilView).D3D11DepthStencilView;
                }

                activeD3D11RenderTargetViews[0] = (view as SdxRenderTargetView).D3D11RenderTargetView;

                D3D11OutputMergerStage.SetTargets(d3d11DepthStencilView, activeD3D11RenderTargetViews[0]);
            }
        }

        protected override void SetRenderTargetViewsCore(RenderTargetView[] views)
        {
            if (views.Length == 0)
                throw new ArgumentException("Invalid size of array: 0", "views");

            if (views[0] == null)
                throw new ArgumentException(string.Format("views[{0}] is null.", 0), "views");

            // 深度ステンシルは先頭のレンダ ターゲットの物を利用。
            var depthStencilView = views[0].DepthStencilView;

            D3D11DepthStencilView d3d11DepthStencilView = null;
            if (depthStencilView != null)
            {
                d3d11DepthStencilView = (depthStencilView as SdxDepthStencilView).D3D11DepthStencilView;
            }

            // TODO
            //
            // MRT の場合に各レンダ ターゲット間の整合性 (サイズ等) を確認すべき。

            // インタフェースの差異の関係上、D3D 実体をコピーして保持。
            for (int i = 0; i < activeD3D11RenderTargetViews.Length; i++)
            {
                if (i < views.Length)
                {
                    if (views[i] == null)
                        throw new ArgumentException(string.Format("views[{0}] is null.", i), "views");

                    activeD3D11RenderTargetViews[i] = (views[i] as SdxRenderTargetView).D3D11RenderTargetView;
                }
                else
                {
                    activeD3D11RenderTargetViews[i] = null;
                }
            }

            D3D11OutputMergerStage.SetTargets(d3d11DepthStencilView, activeD3D11RenderTargetViews);
        }
    }
}
