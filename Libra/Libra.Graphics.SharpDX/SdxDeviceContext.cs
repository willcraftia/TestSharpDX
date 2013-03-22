﻿#region Using

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using D3D11Buffer = SharpDX.Direct3D11.Buffer;
using D3D11CommonShaderStage = SharpDX.Direct3D11.CommonShaderStage;
using D3D11DepthStencilClearFlags = SharpDX.Direct3D11.DepthStencilClearFlags;
using D3D11DepthStencilView = SharpDX.Direct3D11.DepthStencilView;
using D3D11DeviceContext = SharpDX.Direct3D11.DeviceContext;
using D3D11DeviceContextType = SharpDX.Direct3D11.DeviceContextType;
using D3D11MapFlags = SharpDX.Direct3D11.MapFlags;
using D3D11MapMode = SharpDX.Direct3D11.MapMode;
using D3D11PixelShader = SharpDX.Direct3D11.PixelShader;
using D3D11PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology;
using D3D11RenderTargetView = SharpDX.Direct3D11.RenderTargetView;
using D3D11Resource = SharpDX.Direct3D11.Resource;
using D3D11ResourceRegion = SharpDX.Direct3D11.ResourceRegion;
using D3D11SamplerState = SharpDX.Direct3D11.SamplerState;
using D3D11ShaderResourceView = SharpDX.Direct3D11.ShaderResourceView;
using D3D11VertexBufferBinding = SharpDX.Direct3D11.VertexBufferBinding;
using D3D11VertexShader = SharpDX.Direct3D11.VertexShader;
using DXGIFormat = SharpDX.DXGI.Format;
using SDXColor4 = SharpDX.Color4;
using SDXDataBox = SharpDX.DataBox;
using SDXUtilities = SharpDX.Utilities;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxDeviceContext : DeviceContext
    {
        #region ShaderStageState

        sealed class ShaderStageState
        {
            public D3D11Buffer[] ConstantBuffers;

            public D3D11SamplerState[] SamplerStates;

            public D3D11ShaderResourceView[] ShaderResourceViews;

            public ShaderStageState()
            {
                ConstantBuffers = new D3D11Buffer[ConstantBufferSlotCount];
                SamplerStates = new D3D11SamplerState[SamplerSlotCount];
                ShaderResourceViews = new D3D11ShaderResourceView[InputResourceSlotCount];
            }
        }

        #endregion

        SdxDevice device;

        bool deferred;

        D3D11RenderTargetView[] activeD3D11RenderTargetViews;

        ShaderStageState[] shaderStageStates;

        public override bool Deferred
        {
            get { return deferred; }
        }

        public D3D11DeviceContext D3D11DeviceContext { get; private set; }

        public SdxDeviceContext(SdxDevice device, D3D11DeviceContext d3d11DeviceContext)
            : base(device)
        {
            if (device == null) throw new ArgumentNullException("device");
            if (d3d11DeviceContext == null) throw new ArgumentNullException("d3d11DeviceContext");

            this.device = device;
            D3D11DeviceContext = d3d11DeviceContext;

            deferred = (d3d11DeviceContext.TypeInfo == D3D11DeviceContextType.Deferred);

            activeD3D11RenderTargetViews = new D3D11RenderTargetView[SimultaneousRenderTargetCount];

            shaderStageStates = new ShaderStageState[5];
            for (int i = 0; i < shaderStageStates.Length; i++)
            {
                shaderStageStates[i] = new ShaderStageState();
            }
        }

        protected override void OnInputLayoutChanged()
        {
            if (InputLayout == null)
            {
                D3D11DeviceContext.InputAssembler.InputLayout = null;
            }
            else
            {
                D3D11DeviceContext.InputAssembler.InputLayout = (InputLayout as SdxInputLayout).D3D11InputLayout;
            }
        }

        protected override void OnPrimitiveTopologyChanged()
        {
            D3D11DeviceContext.InputAssembler.PrimitiveTopology = (D3D11PrimitiveTopology) PrimitiveTopology;
        }

        protected override void OnIndexBufferChanged()
        {
            var d3d11Buffer = (IndexBuffer as SdxIndexBuffer).D3D11Buffer;

            D3D11DeviceContext.InputAssembler.SetIndexBuffer(d3d11Buffer, (DXGIFormat) IndexBuffer.Format, 0);
        }

        protected override void SetVertexBufferCore(int slot, VertexBufferBinding binding)
        {
            var d3d11VertexBufferBinding = new D3D11VertexBufferBinding
            {
                Buffer = (binding.VertexBuffer as SdxVertexBuffer).D3D11Buffer,
                Offset = binding.Offset,
                Stride = binding.VertexBuffer.VertexDeclaration.Stride
            };

            D3D11DeviceContext.InputAssembler.SetVertexBuffers(slot, d3d11VertexBufferBinding);
        }

        protected override void SetVertexBuffersCore(int startSlot, int count, VertexBufferBinding[] bindings)
        {
            // D3D11 のポインタ渡しインタフェースが公開されているため、
            // stackalloc を利用して配列をヒープに作らずに済む方法もあるが、
            // 将来の SharpDX の更新によりインタフェースが隠蔽される可能性もあるため、
            // 配列複製で対応する。

            var d3d11VertexBufferBindings = new D3D11VertexBufferBinding[count];
            for (int i = 0; i < count; i++)
            {
                d3d11VertexBufferBindings[i] = new D3D11VertexBufferBinding
                {
                    Buffer = (bindings[i].VertexBuffer as SdxVertexBuffer).D3D11Buffer,
                    Offset = bindings[i].Offset,
                    Stride = bindings[i].VertexBuffer.VertexDeclaration.Stride
                };
            }

            D3D11DeviceContext.InputAssembler.SetVertexBuffers(startSlot, d3d11VertexBufferBindings);
        }

        protected override void OnRasterizerStateChanged()
        {
            if (RasterizerState == null)
            {
                D3D11DeviceContext.Rasterizer.State = null;
            }
            else
            {
                D3D11DeviceContext.Rasterizer.State = (Device as SdxDevice).RasterizerStateManager[RasterizerState];
            }
        }

        protected override void OnViewportChanged()
        {
            D3D11DeviceContext.Rasterizer.SetViewports(Viewport.ToSDXViewportF());
        }

        protected override void OnScissorRectangleChanged()
        {
            D3D11DeviceContext.Rasterizer.SetScissorRectangles(ScissorRectangle.ToSDXRectangle());
        }

        protected override void OnBlendStateChanged()
        {
            if (BlendState == null)
            {
                D3D11DeviceContext.OutputMerger.SetBlendState(null, BlendFactor.ToSDXColor4(), -1);
            }
            else
            {
                var d3d11BlendState = (Device as SdxDevice).BlendStateManager[BlendState];
                D3D11DeviceContext.OutputMerger.SetBlendState(
                    d3d11BlendState, BlendState.BlendFactor.ToSDXColor4(), BlendState.MultiSampleMask);
            }
        }

        protected override void OnDepthStencilStateChanged()
        {
            if (DepthStencilState == null)
            {
                D3D11DeviceContext.OutputMerger.SetDepthStencilState(null);
            }
            else
            {
                var d3d11DepthStancilState = (Device as SdxDevice).DepthStencilStateManager[DepthStencilState];

                D3D11DeviceContext.OutputMerger.SetDepthStencilState(
                    d3d11DepthStancilState, DepthStencilState.ReferenceStencil);
            }
        }

        protected override void SetRenderTargetViewCore(RenderTargetView view)
        {
            if (view == null)
            {
                // null 指定の場合はレンダ ターゲットおよび深度ステンシルを外す。
                D3D11DeviceContext.OutputMerger.SetTargets((D3D11DepthStencilView) null, (D3D11RenderTargetView[]) null);
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

                D3D11DeviceContext.OutputMerger.SetTargets(d3d11DepthStencilView, activeD3D11RenderTargetViews[0]);
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

            D3D11DeviceContext.OutputMerger.SetTargets(d3d11DepthStencilView, activeD3D11RenderTargetViews);
        }

        protected override void OnVertexShaderChanged()
        {
            D3D11VertexShader d3d11VertexShader;

            if (VertexShader == null)
            {
                d3d11VertexShader = null;
            }
            else
            {
                d3d11VertexShader = (VertexShader as SdxVertexShader).D3D11VertexShader;
            }

            D3D11DeviceContext.VertexShader.Set(d3d11VertexShader);
        }

        protected override void OnPixelShaderChanged()
        {
            D3D11PixelShader d3d11PixelShader;

            if (PixelShader == null)
            {
                d3d11PixelShader = null;
            }
            else
            {
                d3d11PixelShader = (PixelShader as SdxPixelShader).D3D11PixelShader;
            }

            D3D11DeviceContext.PixelShader.Set(d3d11PixelShader);
        }

        protected override void SetConstantBufferCore(ShaderStage shaderStage, int slot, ConstantBuffer buffer)
        {
            var stageState = shaderStageStates[(int) shaderStage];

            if (buffer == null)
            {
                stageState.ConstantBuffers[slot] = null;
            }
            else
            {
                stageState.ConstantBuffers[slot] = (buffer as SdxConstantBuffer).D3D11Buffer;
            }

            GetD3D11CommonShaderStage(shaderStage).SetConstantBuffer(slot, stageState.ConstantBuffers[slot]);
        }

        protected override void SetConstantBuffersCore(ShaderStage shaderStage, int startSlot, int count, ConstantBuffer[] buffers)
        {
            var stageState = shaderStageStates[(int) shaderStage];

            for (int i = 0; i < count; i++)
            {
                if (buffers[i] == null)
                {
                    stageState.ConstantBuffers[i] = null;
                }
                else
                {
                    stageState.ConstantBuffers[i] = (buffers[i] as SdxConstantBuffer).D3D11Buffer;
                }
            }

            GetD3D11CommonShaderStage(shaderStage).SetConstantBuffers(startSlot, count, stageState.ConstantBuffers);
        }

        protected override void SetSamplerStateCore(ShaderStage shaderStage, int slot, SamplerState state)
        {
            var stageState = shaderStageStates[(int) shaderStage];

            if (state == null)
            {
                stageState.SamplerStates[slot] = null;
            }
            else
            {
                stageState.SamplerStates[slot] = device.SamplerStateManager[state];
            }

            GetD3D11CommonShaderStage(shaderStage).SetSampler(slot, stageState.SamplerStates[slot]);
        }

        protected override void SetSamplerStatesCore(ShaderStage shaderStage, int startSlot, int count, SamplerState[] states)
        {
            var stageState = shaderStageStates[(int) shaderStage];

            for (int i = 0; i < count; i++)
            {
                if (states[i] == null)
                {
                    stageState.SamplerStates[i] = null;
                }
                else
                {
                    stageState.SamplerStates[i] = device.SamplerStateManager[states[i]];
                }
            }

            GetD3D11CommonShaderStage(shaderStage).SetSamplers(startSlot, count, stageState.SamplerStates);
        }

        protected override void SetShaderResourceViewCore(ShaderStage shaderStage, int slot, ShaderResourceView view)
        {
            var stageState = shaderStageStates[(int) shaderStage];

            if (view == null)
            {
                stageState.ShaderResourceViews[slot] = null;
            }
            else
            {
                stageState.ShaderResourceViews[slot] = (view as SdxShaderResourceView).D3D11ShaderResourceView;
            }

            GetD3D11CommonShaderStage(shaderStage).SetShaderResource(slot, stageState.ShaderResourceViews[slot]);
        }

        protected override void SetShaderResourceViewsCore(ShaderStage shaderStage, int startSlot, int count, ShaderResourceView[] views)
        {
            var stageState = shaderStageStates[(int) shaderStage];

            for (int i = 0; i < count; i++)
            {
                if (views[i] == null)
                {
                    stageState.ShaderResourceViews[i] = null;
                }
                else
                {
                    stageState.ShaderResourceViews[i] = (views[i] as SdxShaderResourceView).D3D11ShaderResourceView;
                }
            }

            GetD3D11CommonShaderStage(shaderStage).SetShaderResources(startSlot, count, stageState.ShaderResourceViews);
        }

        D3D11CommonShaderStage GetD3D11CommonShaderStage(ShaderStage shaderStage)
        {
            switch (shaderStage)
            {
                case ShaderStage.Vertex:
                    return D3D11DeviceContext.VertexShader;
                case ShaderStage.Hull:
                    return D3D11DeviceContext.HullShader;
                case ShaderStage.Domain:
                    return D3D11DeviceContext.DomainShader;
                case ShaderStage.Geometry:
                    return D3D11DeviceContext.GeometryShader;
                case ShaderStage.Pixel:
                    return D3D11DeviceContext.PixelShader;
            }

            throw new ArgumentException("Unknown shader stage: " + shaderStage, "shaderStage");
        }

        public override void ClearRenderTargetView(RenderTargetView view, ClearOptions options, Vector4 color, float depth, byte stencil)
        {
            if (view == null) throw new ArgumentNullException("view");

            if ((options & ClearOptions.Target) != 0)
            {
                var d3d11RenderTargetView = (view as SdxRenderTargetView).D3D11RenderTargetView;

                D3D11DeviceContext.ClearRenderTargetView(
                    d3d11RenderTargetView, new SDXColor4(color.X, color.Y, color.Z, color.W));
            }

            var depthStencilView = view.DepthStencilView;
            if (depthStencilView == null)
                return;

            D3D11DepthStencilClearFlags flags = 0;

            if ((options & ClearOptions.Depth) != 0)
                flags |= D3D11DepthStencilClearFlags.Depth;

            if ((options & ClearOptions.Stencil) != 0)
                flags |= D3D11DepthStencilClearFlags.Stencil;

            if (flags != 0)
            {
                var d3d11DepthStencilView = (depthStencilView as SdxDepthStencilView).D3D11DepthStencilView;

                D3D11DeviceContext.ClearDepthStencilView(d3d11DepthStencilView, flags, depth, stencil);
            }
        }

        protected override void DrawCore(int vertexCount, int startVertexLocation)
        {
            D3D11DeviceContext.Draw(vertexCount, startVertexLocation);
        }

        protected override void DrawIndexedCore(int indexCount, int startIndexLocation, int baseVertexLocation)
        {
            D3D11DeviceContext.DrawIndexed(indexCount, startIndexLocation, baseVertexLocation);
        }

        protected override MappedSubresource Map(Resource resource, int subresource, MapMode mapMode)
        {
            var d3d11Resource = GetD3D11Resource(resource);
            var dataBox = D3D11DeviceContext.MapSubresource(d3d11Resource, subresource, (D3D11MapMode) mapMode, D3D11MapFlags.None);
            return new MappedSubresource(dataBox.DataPointer, dataBox.RowPitch, dataBox.SlicePitch);
        }

        protected override void Unmap(Resource resource, int subresource)
        {
            var d3d11Resource = GetD3D11Resource(resource);
            D3D11DeviceContext.UnmapSubresource(d3d11Resource, subresource);
        }

        protected override void UpdateSubresource(
            Resource destinationResource, int destinationSubresource, Box? destinationBox,
            IntPtr sourcePointer, int sourceRowPitch, int sourceDepthPitch)
        {
            var d3d11Resource = GetD3D11Resource(destinationResource);
            D3D11ResourceRegion? d3d11ResourceRegion = null;
            if (destinationBox.HasValue)
            {
                d3d11ResourceRegion = new D3D11ResourceRegion
                {
                    Left = destinationBox.Value.Left,
                    Top = destinationBox.Value.Top,
                    Front = destinationBox.Value.Front,
                    Right = destinationBox.Value.Right,
                    Bottom = destinationBox.Value.Bottom,
                    Back = destinationBox.Value.Back
                };
            }
            D3D11DeviceContext.UpdateSubresource(
                d3d11Resource, destinationSubresource, d3d11ResourceRegion, sourcePointer, sourceRowPitch, sourceDepthPitch);
        }

        D3D11Resource GetD3D11Resource(Resource resource)
        {
            var constantBuffer = resource as SdxConstantBuffer;
            if (constantBuffer != null)
                return constantBuffer.D3D11Buffer;

            var vertexBuffer = resource as SdxVertexBuffer;
            if (vertexBuffer != null)
                return vertexBuffer.D3D11Buffer;

            var texture2D = resource as SdxTexture2D;
            if (texture2D != null)
                return texture2D.D3D11Texture2D;

            throw new ArgumentException("Unknown resource specified: " + resource.GetType(), "resource");
        }

        #region IDisposable

        protected override void DisposeOverride(bool disposing)
        {
            if (disposing)
            {
                D3D11DeviceContext.Dispose();
            }

            base.DisposeOverride(disposing);
        }

        #endregion
    }
}
