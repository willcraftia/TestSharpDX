#region Using

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using D3D11Buffer = SharpDX.Direct3D11.Buffer;
using D3D11CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags;
using D3D11DeviceChild = SharpDX.Direct3D11.DeviceChild;
using D3D11MapFlags = SharpDX.Direct3D11.MapFlags;
using D3D11MapMode = SharpDX.Direct3D11.MapMode;
using D3D11Resource = SharpDX.Direct3D11.Resource;
using D3D11ResourceDimension = SharpDX.Direct3D11.ResourceDimension;
using D3D11ResourceUsage = SharpDX.Direct3D11.ResourceUsage;
using D3D11Texture1D = SharpDX.Direct3D11.Texture1D;
using D3D11Texture2D = SharpDX.Direct3D11.Texture2D;
using D3D11Texture3D = SharpDX.Direct3D11.Texture3D;
using SDXDataBox = SharpDX.DataBox;
using SDXUtilities = SharpDX.Utilities;

#endregion

namespace Libra.Graphics.SharpDX
{
    public class SdxResource : IResource
    {
        public event EventHandler Disposing;

        public ResourceUsage Usage { get; private set; }

        public string Name { get; set; }

        public object Tag { get; set; }

        public D3D11Resource D3D11Resource { get; private set; }

        protected SdxResource(D3D11Resource d3d11Resource)
        {
            if (d3d11Resource == null) throw new ArgumentNullException("d3d11Resource");
            if (d3d11Resource.Dimension == D3D11ResourceDimension.Unknown)
                throw new ArgumentException("Unknown dimension not supported.", "d3d11Resource");

            D3D11Resource = d3d11Resource;

            switch (D3D11Resource.Dimension)
            {
                case D3D11ResourceDimension.Buffer:
                    Usage = (ResourceUsage) (D3D11Resource as D3D11Buffer).Description.Usage;
                    break;
                case D3D11ResourceDimension.Texture1D:
                    Usage = (ResourceUsage) (D3D11Resource as D3D11Texture1D).Description.Usage;
                    break;
                case D3D11ResourceDimension.Texture2D:
                    Usage = (ResourceUsage) (D3D11Resource as D3D11Texture2D).Description.Usage;
                    break;
                case D3D11ResourceDimension.Texture3D:
                    Usage = (ResourceUsage) (D3D11Resource as D3D11Texture3D).Description.Usage;
                    break;
            }
        }

        internal static D3D11CpuAccessFlags ResolveD3D11CpuAccessFlags(D3D11ResourceUsage usage)
        {
            if (usage == D3D11ResourceUsage.Staging)
                return D3D11CpuAccessFlags.Read | D3D11CpuAccessFlags.Write;

            if (usage == D3D11ResourceUsage.Dynamic)
                return D3D11CpuAccessFlags.Write;

            return D3D11CpuAccessFlags.None;
        }

        public void GetData<T>(IDeviceContext context, int level, T[] data, int startIndex, int elementCount) where T : struct
        {
            if (Usage != ResourceUsage.Staging)
                throw new InvalidOperationException("Data can not be get from CPU.");

            // アドレスを固定。
            var gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                var dataPointer = gcHandle.AddrOfPinnedObject();
                var sizeOfT = SdxUtilities.SizeOf<T>();
                var sizeInBytesOfData = ((elementCount == 0) ? data.Length : elementCount) * sizeOfT;
                GetData(context, level, dataPointer, sizeInBytesOfData);
            }
            finally
            {
                gcHandle.Free();
            }
        }

        void GetData(IDeviceContext context, int level, IntPtr destinationPtr, int sizeInBytes)
        {
            // Map
            SDXDataBox dataBox;
            Map(context, level, D3D11MapMode.Read, out dataBox);
            try
            {
                SDXUtilities.CopyMemory(destinationPtr, dataBox.DataPointer, sizeInBytes);
            }
            finally
            {
                // Unmap
                Unmap(context, level);
            }
        }

        public void SetData<T>(IDeviceContext context, params T[] data) where T : struct
        {
            SetData(context, data, 0, data.Length);
        }

        public void SetData<T>(IDeviceContext context, T[] data, int startIndex, int elementCount) where T : struct
        {
            if (context == null) throw new ArgumentNullException("context");
            if (data == null) throw new ArgumentNullException("data");

            if (Usage == ResourceUsage.Immutable)
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
                    SetData(context, sourcePointer, sizeInBytesOfData);
                }
            }
            finally
            {
                // アドレス固定を解放。
                gcHandle.Free();
            }
        }

        void SetData(IDeviceContext context, IntPtr sourcePointer, int sizeInBytes)
        {
            if (Usage == ResourceUsage.Default)
            {
                // Immutable と Dynamic 以外は UpdateSubresource で更新可能。
                // Staging は Map/Unmap で行えるので、Default の場合にのみ UpdateSubresource で更新。
                var d3d11DeviceContext = (context as SdxDeviceContext).D3D11DeviceContext;
                d3d11DeviceContext.UpdateSubresource(new SDXDataBox(sourcePointer), D3D11Resource);
            }
            else
            {
                // Map
                SDXDataBox dataBox;
                Map(context, 0, D3D11MapMode.WriteDiscard, out dataBox);
                try
                {
                    SDXUtilities.CopyMemory(dataBox.DataPointer, sourcePointer, sizeInBytes);
                }
                finally
                {
                    // Unmap
                    Unmap(context, 0);
                }
            }
        }

        void Map(IDeviceContext context, int subresource, D3D11MapMode mapType, out SDXDataBox result)
        {
            var d3d11DeviceContext = (context as SdxDeviceContext).D3D11DeviceContext;
            result = d3d11DeviceContext.MapSubresource(D3D11Resource, subresource, mapType, D3D11MapFlags.None);
        }

        void Unmap(IDeviceContext context, int subresource)
        {
            var d3d11DeviceContext = (context as SdxDeviceContext).D3D11DeviceContext;
            d3d11DeviceContext.UnmapSubresource(D3D11Resource, 0);
        }

        #region ToString

        public override string ToString()
        {
            if (Name != null)
                return "Name=" + Name;

            return base.ToString();
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; private set; }

        ~SdxResource()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void DisposeOverride(bool disposing) { }

        void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            if (Disposing != null)
                Disposing(this, EventArgs.Empty);

            DisposeOverride(disposing);

            D3D11Resource.Dispose();

            IsDisposed = true;
        }

        #endregion
    }
}
