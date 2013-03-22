#region Using

using System;
using System.Runtime.InteropServices;

using D3D11DepthStencilClearFlags = SharpDX.Direct3D11.DepthStencilClearFlags;
using D3D11DeviceContext = SharpDX.Direct3D11.DeviceContext;
using D3D11DeviceContextType = SharpDX.Direct3D11.DeviceContextType;
using D3D11MapFlags = SharpDX.Direct3D11.MapFlags;
using D3D11MapMode = SharpDX.Direct3D11.MapMode;
using D3D11Resource = SharpDX.Direct3D11.Resource;
using D3D11ResourceRegion = SharpDX.Direct3D11.ResourceRegion;
using SDXColor4 = SharpDX.Color4;
using SDXDataBox = SharpDX.DataBox;
using SDXUtilities = SharpDX.Utilities;

#endregion

namespace Libra.Graphics.SharpDX
{
    public sealed class SdxDeviceContext : DeviceContext
    {
        SdxDevice device;

        bool deferred;

        SdxInputAssemblerStage inputAssemblerStage;

        SdxVertexShaderStage vertexShaderStage;

        SdxRasterizerStage rasterizerStage;

        SdxPixelShaderStage pixelShaderStage;

        SdxOutputMergerStage outputMergerStage;

        public override IDevice Device
        {
            get { return device; }
        }

        public override bool Deferred
        {
            get { return deferred; }
        }

        public override InputAssemblerStage InputAssemblerStage
        {
            get { return inputAssemblerStage; }
        }

        public override VertexShaderStage VertexShaderStage
        {
            get { return vertexShaderStage; }
        }

        public override RasterizerStage RasterizerStage
        {
            get { return rasterizerStage; }
        }

        public override PixelShaderStage PixelShaderStage
        {
            get { return pixelShaderStage; }
        }

        public override OutputMergerStage OutputMergerStage
        {
            get { return outputMergerStage; }
        }

        public D3D11DeviceContext D3D11DeviceContext { get; private set; }

        public SdxDeviceContext(SdxDevice device, D3D11DeviceContext d3d11DeviceContext)
        {
            if (device == null) throw new ArgumentNullException("device");
            if (d3d11DeviceContext == null) throw new ArgumentNullException("d3d11DeviceContext");

            this.device = device;
            D3D11DeviceContext = d3d11DeviceContext;

            deferred = (d3d11DeviceContext.TypeInfo == D3D11DeviceContextType.Deferred);

            // パイプライン ステージの初期化。
            inputAssemblerStage = new SdxInputAssemblerStage(this);
            vertexShaderStage = new SdxVertexShaderStage(this);
            rasterizerStage = new SdxRasterizerStage(this);
            pixelShaderStage = new SdxPixelShaderStage(this);
            outputMergerStage = new SdxOutputMergerStage(this);
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
