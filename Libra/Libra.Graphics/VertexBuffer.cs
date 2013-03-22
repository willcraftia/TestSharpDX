﻿#region Using

using System;
using System.Runtime.InteropServices;

#endregion

namespace Libra.Graphics
{
    public abstract class VertexBuffer : Resource
    {
        public VertexDeclaration VertexDeclaration { get; private set; }

        public int VertexCount { get; private set; }

        protected VertexBuffer() { }

        public void Initialize(VertexDeclaration vertexDeclaration, int vertexCount)
        {
            if (vertexDeclaration == null) throw new ArgumentNullException("vertexDeclaration");
            if (vertexCount < 1) throw new ArgumentOutOfRangeException("vertexCount");

            if (Usage == ResourceUsage.Immutable)
                throw new InvalidOperationException("Usage must be not immutable.");

            VertexDeclaration = vertexDeclaration;
            VertexCount = vertexCount;

            InitializeCore();
        }

        public void Initialize<T>(VertexDeclaration vertexDeclaration, T[] data) where T : struct
        {
            if (vertexDeclaration == null) throw new ArgumentNullException("vertexDeclaration");
            if (data.Length == 0) throw new ArgumentException("Data must be not empty.", "data");

            VertexDeclaration = vertexDeclaration;
            VertexCount = Marshal.SizeOf(typeof(T)) * data.Length / vertexDeclaration.Stride;

            InitializeCore(data);
        }

        public void Initialize<T>(T[] data) where T : struct, IVertexType
        {
            if (data.Length == 0) throw new ArgumentException("Data must be not empty.", "data");

            VertexDeclaration = data[0].VertexDeclaration;
            VertexCount = data.Length;

            InitializeCore(data);
        }

        protected abstract void InitializeCore();

        protected abstract void InitializeCore<T>(T[] data) where T : struct;

        public abstract void GetData<T>(DeviceContext context, T[] data, int startIndex, int elementCount) where T : struct;

        public void GetData<T>(DeviceContext context, T[] data) where T : struct
        {
            GetData(context, data, 0, data.Length);
        }

        public void SetData<T>(DeviceContext context, T[] data, int startIndex, int elementCount) where T : struct
        {
            if (data == null) throw new ArgumentNullException("data");

            if (Usage == ResourceUsage.Immutable)
                throw new InvalidOperationException("Data can not be set from CPU.");

            var gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                var dataPointer = gcHandle.AddrOfPinnedObject();
                var sizeOfT = Marshal.SizeOf(typeof(T));

                var sourcePointer = (IntPtr) (dataPointer + startIndex * sizeOfT);

                if (Usage == ResourceUsage.Default)
                {
                    context.UpdateSubresource(this, 0, null, sourcePointer, 0, 0);
                }
                else
                {
                    var sizeInBytes = ((elementCount == 0) ? data.Length : elementCount) * sizeOfT;

                    // TODO
                    //
                    // Dynamic だと D3D11MapMode.Write はエラーになる。
                    // 対応関係を MSDN から把握できないが、どうすべきか。
                    // ひとまず WriteDiscard とする。

                    var mappedResource = context.Map(this, 0, DeviceContext.MapMode.WriteDiscard);
                    try
                    {
                        GraphicsHelper.CopyMemory(mappedResource.Pointer, sourcePointer, sizeInBytes);
                    }
                    finally
                    {
                        context.Unmap(this, 0);
                    }
                }
            }
            finally
            {
                gcHandle.Free();
            }
        }

        public void SetData<T>(DeviceContext context, params T[] data) where T : struct
        {
            SetData(context, data, 0, data.Length);
        }

        public void SetData<T>(DeviceContext context, T[] data, int sourceIndex, int elementCount,
            int destinationIndex, SetDataOptions options = SetDataOptions.None) where T : struct
        {
            if (Usage != ResourceUsage.Dynamic)
                throw new InvalidOperationException("Resource not writable.");

            if (options == SetDataOptions.Discard && Usage != ResourceUsage.Dynamic)
                throw new InvalidOperationException("Resource.Usage must be dynamic for discard option.");

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

                    var mappedResource = context.Map(this, 0, (DeviceContext.MapMode) options);
                    var destinationPtr = (IntPtr) (mappedResource.Pointer + destinationIndex * sizeOfT);

                    try
                    {
                        GraphicsHelper.CopyMemory(destinationPtr, sourcePointer, sizeInBytes);
                    }
                    finally
                    {
                        // Unmap
                        context.Unmap(this, 0);
                    }
                }
            }
            finally
            {
                gcHandle.Free();
            }
        }
    }
}
