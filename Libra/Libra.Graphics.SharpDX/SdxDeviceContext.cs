#region Using

using System;
using System.Runtime.InteropServices;

using D3D11DepthStencilClearFlags = SharpDX.Direct3D11.DepthStencilClearFlags;
using D3D11DeviceContext = SharpDX.Direct3D11.DeviceContext;
using D3D11DeviceContextType = SharpDX.Direct3D11.DeviceContextType;
using D3D11MapFlags = SharpDX.Direct3D11.MapFlags;
using D3D11MapMode = SharpDX.Direct3D11.MapMode;
using D3D11Resource = SharpDX.Direct3D11.Resource;
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
            inputAssemblerStage = new SdxInputAssemblerStage(d3d11DeviceContext.InputAssembler);
            vertexShaderStage = new SdxVertexShaderStage(device, d3d11DeviceContext.VertexShader);
            rasterizerStage = new SdxRasterizerStage(device, d3d11DeviceContext.Rasterizer);
            pixelShaderStage = new SdxPixelShaderStage(device, d3d11DeviceContext.PixelShader);
            outputMergerStage = new SdxOutputMergerStage(device, this, d3d11DeviceContext.OutputMerger);
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

        public override void Draw(int vertexCount, int startVertexLocation = 0)
        {
            D3D11DeviceContext.Draw(vertexCount, startVertexLocation);
        }

        public override void DrawIndexed(int indexCount, int startIndexLocation = 0, int baseVertexLocation = 0)
        {
            D3D11DeviceContext.DrawIndexed(indexCount, startIndexLocation, baseVertexLocation);
        }

        public override void GetData<T>(Resource resource, int level, T[] data, int startIndex, int elementCount)
        {
            if (resource == null) throw new ArgumentNullException("resource");
            if (data == null) throw new ArgumentNullException("data");

            if (resource.Usage != ResourceUsage.Staging)
                throw new InvalidOperationException("Data can not be get from CPU.");

            // アドレスを固定。
            var gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                unsafe
                {
                    var dataPointer = gcHandle.AddrOfPinnedObject();
                    var sizeOfT = SdxUtilities.SizeOf<T>();
                    var destinationPtr = (IntPtr) ((byte*) dataPointer + startIndex * sizeOfT);
                    var sizeInBytes = ((elementCount == 0) ? data.Length : elementCount) * sizeOfT;
                    GetData(resource, level, destinationPtr, sizeInBytes);
                }
            }
            finally
            {
                gcHandle.Free();
            }
        }

        D3D11Resource GetD3D11Resource(Resource resource)
        {
            var constantBuffer = resource as SdxConstantBuffer;
            if (constantBuffer != null)
                return constantBuffer.D3D11Buffer;

            var vertexBuffer = resource as SdxVertexBuffer;
            if (vertexBuffer != null)
                return vertexBuffer.D3D11Buffer;

            throw new ArgumentException("Unknown resource specified: " + resource.GetType(), "resource");
        }

        void GetData(Resource resource, int level, IntPtr destinationPtr, int sizeInBytes)
        {
            if (resource == null) throw new ArgumentNullException("resource");

            var d3d11Resource = GetD3D11Resource(resource);

            // Map
            var dataBox = D3D11DeviceContext.MapSubresource(d3d11Resource, level, D3D11MapMode.Read, D3D11MapFlags.None);
            try
            {
                SDXUtilities.CopyMemory(destinationPtr, dataBox.DataPointer, sizeInBytes);
            }
            finally
            {
                // Unmap
                D3D11DeviceContext.UnmapSubresource(d3d11Resource, level);
            }
        }

        public override void SetData<T>(Resource resource, T[] data, int startIndex, int elementCount)
        {
            if (resource == null) throw new ArgumentNullException("resource");
            if (data == null) throw new ArgumentNullException("data");

            if (resource.Usage == ResourceUsage.Immutable)
                throw new InvalidOperationException("Data can not be set from CPU.");

            // TODO
            // width * height との比較で範囲検査。

            // アドレスを固定。
            var gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                var dataPointer = gcHandle.AddrOfPinnedObject();
                var sizeOfT = SdxUtilities.SizeOf<T>();

                unsafe
                {
                    var sourcePointer = (IntPtr) ((byte*) dataPointer + startIndex * sizeOfT);
                    var sizeInBytesOfData = ((elementCount == 0) ? data.Length : elementCount) * sizeOfT;
                    SetData(resource, sourcePointer, sizeInBytesOfData);
                }
            }
            finally
            {
                // アドレス固定を解放。
                gcHandle.Free();
            }
        }

        void SetData(Resource resource, IntPtr sourcePointer, int sizeInBytes)
        {
            var d3d11Resource = GetD3D11Resource(resource);

            if (resource.Usage == ResourceUsage.Default)
            {
                // Immutable と Dynamic 以外は UpdateSubresource で更新可能。
                // Staging は Map/Unmap で行えるので、Default の場合にのみ UpdateSubresource で更新。
                D3D11DeviceContext.UpdateSubresource(new SDXDataBox(sourcePointer), d3d11Resource);
            }
            else
            {
                // Map
                var dataBox = D3D11DeviceContext.MapSubresource(d3d11Resource, 0, D3D11MapMode.Write, D3D11MapFlags.None);
                try
                {
                    SDXUtilities.CopyMemory(dataBox.DataPointer, sourcePointer, sizeInBytes);
                }
                finally
                {
                    // Unmap
                    D3D11DeviceContext.UnmapSubresource(d3d11Resource, 0);
                }
            }
        }

        public override void SetData<T>(Resource resource, T[] data, int sourceIndex, int elementCount,
            int destinationIndex, SetDataOptions options = SetDataOptions.None)
        {
            if (resource.Usage != ResourceUsage.Dynamic && resource.Usage != ResourceUsage.Staging)
                throw new InvalidOperationException("Resource not writable.");

            if (options == SetDataOptions.Discard && resource.Usage != ResourceUsage.Dynamic)
                throw new InvalidOperationException("Resource.Usage must be dynamic for discard option.");

            if ((options == SetDataOptions.Discard || options == SetDataOptions.NoOverwrite) &&
                resource is ConstantBuffer)
                throw new InvalidOperationException("Resource must be not a constant buffer for discard/no overwite option.");

            var gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                var dataPointer = gcHandle.AddrOfPinnedObject();
                var sizeOfT = SdxUtilities.SizeOf<T>();

                unsafe
                {
                    var sourcePointer = (IntPtr) ((byte*) dataPointer + sourceIndex * sizeOfT);
                    var sizeInBytes = ((elementCount == 0) ? data.Length : elementCount) * sizeOfT;

                    var d3d11Resource = GetD3D11Resource(resource);
                    var d3d11MapMode = (D3D11MapMode) options;

                    // メモ
                    //
                    // D3D11MapFlags.DoNotWait は、Discard と NoOverwite では使えない。
                    // D3D11MapFlags 参照のこと。

                    var dataBox = D3D11DeviceContext.MapSubresource(d3d11Resource, 0, d3d11MapMode, D3D11MapFlags.None);
                    var destinationPtr = (IntPtr) ((byte*) dataBox.DataPointer + destinationIndex * sizeOfT);

                    try
                    {
                        SDXUtilities.CopyMemory(destinationPtr, sourcePointer, sizeInBytes);
                    }
                    finally
                    {
                        // Unmap
                        D3D11DeviceContext.UnmapSubresource(d3d11Resource, 0);
                    }
                }
            }
            finally
            {
                gcHandle.Free();
            }
        }

        protected override IntPtr Map(Resource resource, int subresource, DeviceContext.MapMode mapMode)
        {
            var d3d11Resource = GetD3D11Resource(resource);
            var dataBox = D3D11DeviceContext.MapSubresource(d3d11Resource, subresource, (D3D11MapMode) mapMode, D3D11MapFlags.None);
            return dataBox.DataPointer;
        }

        protected override void Unmap(Resource resource, int subresource)
        {
            var d3d11Resource = GetD3D11Resource(resource);
            D3D11DeviceContext.UnmapSubresource(d3d11Resource, subresource);
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
