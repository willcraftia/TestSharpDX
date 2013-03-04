#region Using

using System;
using System.Runtime.InteropServices;

using D3D11DeviceContext = SharpDX.Direct3D11.DeviceContext;
using D3D11DeviceContextType = SharpDX.Direct3D11.DeviceContextType;
using D3D11MapFlags = SharpDX.Direct3D11.MapFlags;
using D3D11MapMode = SharpDX.Direct3D11.MapMode;
using D3D11Resource = SharpDX.Direct3D11.Resource;
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

        public override void Draw(int vertexCount, int startVertexLocation = 0)
        {
            D3D11DeviceContext.Draw(vertexCount, startVertexLocation);
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
                var dataBox = D3D11DeviceContext.MapSubresource(d3d11Resource, 0, D3D11MapMode.WriteDiscard, D3D11MapFlags.None);
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
