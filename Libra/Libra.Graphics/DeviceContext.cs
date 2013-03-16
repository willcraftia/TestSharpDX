#region Using

using System;
using System.Runtime.InteropServices;

#endregion

namespace Libra.Graphics
{
    public abstract class DeviceContext : IDisposable
    {
        #region DllImport

        [DllImport("kernel32.dll")]
        static extern void CopyMemory(IntPtr destinationPointer, IntPtr sourcePointer, int size);

        #endregion

        #region MapMode

        internal protected enum MapMode
        {
            Read                = 1,
            Write               = 2,
            ReadWrite           = 3,
            WriteDiscard        = 4,
            WriteNoOverwrite    = 5,
        }

        #endregion

        public event EventHandler Disposing;

        public abstract IDevice Device { get; }

        public abstract bool Deferred { get; }

        public abstract InputAssemblerStage InputAssemblerStage { get; }

        public abstract VertexShaderStage VertexShaderStage { get; }

        public abstract RasterizerStage RasterizerStage { get; }

        public abstract PixelShaderStage PixelShaderStage { get; }

        public abstract OutputMergerStage OutputMergerStage { get; }

        public void ClearRenderTargetView(
            RenderTargetView view, ClearOptions options, Color color, float depth, byte stencil)
        {
            ClearRenderTargetView(view, options, color.ToVector4(), depth, stencil);
        }

        public abstract void ClearRenderTargetView(
            RenderTargetView view, ClearOptions options, Vector4 color, float depth, byte stencil);

        public void Clear(Color color)
        {
            Clear(color.ToVector4());
        }

        public void Clear(Vector4 color)
        {
            Clear(ClearOptions.Target | ClearOptions.Depth | ClearOptions.Stencil, color);
        }

        public void Clear(ClearOptions options, Color color, float depth = 1f, byte stencil = 0)
        {
            Clear(options, color.ToVector4(), depth, stencil);
        }

        public void Clear(ClearOptions options, Vector4 color, float depth = 1f, byte stencil = 0)
        {
            var renderTargetViews = OutputMergerStage.RenderTargetViews;
            var count = renderTargetViews.Count;

            // アクティブに設定されている全てのレンダ ターゲットをクリア。
            for (int i = 0; i < count; i++)
            {
                var renderTarget = renderTargetViews[i];
                if (renderTarget != null)
                {
                    ClearRenderTargetView(renderTarget, options, color, depth, stencil);
                }
            }
        }

        public abstract void Draw(int vertexCount, int startVertexLocation = 0);

        public abstract void DrawIndexed(int indexCount, int startIndexLocation = 0, int baseVertexLocation = 0);

        internal protected abstract IntPtr Map(Resource resource, int subresource, MapMode mapMode);

        internal protected abstract void Unmap(Resource resource, int subresource);

        internal protected abstract void UpdateSubresource(IntPtr sourcePointer, Resource resource, int subresource);

        internal void GetData<T>(Resource resource, int subresource, T[] data, int startIndex, int elementCount) where T : struct
        {
            if (resource == null) throw new ArgumentNullException("resource");
            if (data == null) throw new ArgumentNullException("data");

            if (resource.Usage != ResourceUsage.Staging)
                throw new InvalidOperationException("Data can not be get from CPU.");

            var gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                unsafe
                {
                    var dataPointer = gcHandle.AddrOfPinnedObject();
                    var sizeOfT = Marshal.SizeOf(typeof(T));
                    var destinationPointer = (IntPtr) ((byte*) dataPointer + startIndex * sizeOfT);
                    var sizeInBytes = ((elementCount == 0) ? data.Length : elementCount) * sizeOfT;

                    var sourcePointer = Map(resource, subresource, MapMode.Read);
                    try
                    {
                        CopyMemory(destinationPointer, sourcePointer, sizeInBytes);
                    }
                    finally
                    {
                        Unmap(resource, subresource);
                    }
                }
            }
            finally
            {
                gcHandle.Free();
            }
        }

        internal void SetData<T>(Resource resource, int subresource, T data) where T : struct
        {
            if (resource == null) throw new ArgumentNullException("resource");

            if (resource.Usage == ResourceUsage.Immutable)
                throw new InvalidOperationException("Data can not be set from CPU.");

            var gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                var sourcePointer = gcHandle.AddrOfPinnedObject();
                var sizeInBytes = Marshal.SizeOf(typeof(T));

                unsafe
                {
                    if (resource.Usage == ResourceUsage.Default)
                    {
                        // TODO
                        //
                        // Immutable と Dynamic 以外は UpdateSubresource で更新可能。
                        // Staging は Map/Unmap で行えるので、Default の場合にのみ UpdateSubresource で更新。
                        // それで良いのか？
                        UpdateSubresource(sourcePointer, resource, subresource);
                    }
                    else
                    {
                        // TODO
                        //
                        // Dynamic だと D3D11MapMode.Write はエラーになる。
                        // 対応関係を MSDN から把握できないが、どうすべきか。
                        // ひとまず WriteDiscard とする。

                        var destinationPointer = Map(resource, subresource, MapMode.WriteDiscard);
                        try
                        {
                            CopyMemory(destinationPointer, sourcePointer, sizeInBytes);
                        }
                        finally
                        {
                            Unmap(resource, subresource);
                        }
                    }
                }
            }
            finally
            {
                gcHandle.Free();
            }
        }

        internal void SetData<T>(Resource resource, int subresource, T[] data, int startIndex, int elementCount) where T : struct
        {
            if (resource == null) throw new ArgumentNullException("resource");
            if (data == null) throw new ArgumentNullException("data");

            if (resource.Usage == ResourceUsage.Immutable)
                throw new InvalidOperationException("Data can not be set from CPU.");

            var gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                var dataPointer = gcHandle.AddrOfPinnedObject();
                var sizeOfT = Marshal.SizeOf(typeof(T));

                unsafe
                {
                    var sourcePointer = (IntPtr) ((byte*) dataPointer + startIndex * sizeOfT);
                    var sizeInBytes = ((elementCount == 0) ? data.Length : elementCount) * sizeOfT;

                    if (resource.Usage == ResourceUsage.Default)
                    {
                        // TODO
                        //
                        // Immutable と Dynamic 以外は UpdateSubresource で更新可能。
                        // Staging は Map/Unmap で行えるので、Default の場合にのみ UpdateSubresource で更新。
                        // それで良いのか？
                        UpdateSubresource(sourcePointer, resource, subresource);
                    }
                    else
                    {
                        // TODO
                        //
                        // Dynamic だと D3D11MapMode.Write はエラーになる。
                        // 対応関係を MSDN から把握できないが、どうすべきか。
                        // ひとまず WriteDiscard とする。

                        var destinationPointer = Map(resource, subresource, MapMode.WriteDiscard);
                        try
                        {
                            CopyMemory(destinationPointer, sourcePointer, sizeInBytes);
                        }
                        finally
                        {
                            Unmap(resource, subresource);
                        }
                    }
                }
            }
            finally
            {
                gcHandle.Free();
            }
        }

        internal void SetData<T>(
            Resource resource, int subresource,
            T[] data, int sourceIndex, int elementCount,
            int destinationIndex, SetDataOptions options = SetDataOptions.None) where T : struct
        {
            if (subresource < 0) throw new ArgumentOutOfRangeException("subresource");

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
                var sizeOfT = Marshal.SizeOf(typeof(T));

                unsafe
                {
                    var sourcePointer = (IntPtr) ((byte*) dataPointer + sourceIndex * sizeOfT);
                    var sizeInBytes = ((elementCount == 0) ? data.Length : elementCount) * sizeOfT;

                    // メモ
                    //
                    // D3D11MapFlags.DoNotWait は、Discard と NoOverwite では使えない。
                    // D3D11MapFlags 参照のこと。

                    var mappedPointer = Map(resource, subresource, (MapMode) options);
                    var destinationPtr = (IntPtr) ((byte*) mappedPointer + destinationIndex * sizeOfT);

                    try
                    {
                        CopyMemory(destinationPtr, sourcePointer, sizeInBytes);
                    }
                    finally
                    {
                        // Unmap
                        Unmap(resource, subresource);
                    }
                }
            }
            finally
            {
                gcHandle.Free();
            }
        }

        protected virtual void OnDisposing(object sender, EventArgs e)
        {
            if (Disposing != null)
                Disposing(sender, e);
        }

        #region IDisposable

        public bool IsDisposed { get; private set; }

        ~DeviceContext()
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

            OnDisposing(this, EventArgs.Empty);

            DisposeOverride(disposing);

            IsDisposed = true;
        }

        #endregion
    }
}
