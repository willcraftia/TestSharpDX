#region Using

using System;
using System.Runtime.InteropServices;

#endregion

namespace Libra.Graphics
{
    public abstract class ConstantBuffer : Resource
    {
        protected ConstantBuffer(IDevice device)
            : base(device)
        {
        }

        public void Initialize(int byteWidth)
        {
            if (byteWidth < 1) throw new ArgumentOutOfRangeException("byteWidth");
            if ((byteWidth % 16) != 0) throw new ArgumentException("byteWidth must be a multiple of 16", "byteWidth");
            if (Usage == ResourceUsage.Immutable)
                throw new InvalidOperationException("Usage must be not immutable.");
        }

        protected abstract void InitializeCore(int byteWidth);

        public void Initialize<T>() where T : struct
        {
            Initialize(Marshal.SizeOf(typeof(T)));
        }

        public void Initialize<T>(T data) where T : struct
        {
            Initialize<T>(Marshal.SizeOf(typeof(T)), data);
        }

        public void Initialize<T>(int byteWidth, T data) where T : struct
        {
            if (byteWidth < 1) throw new ArgumentOutOfRangeException("byteWidth");
            if ((byteWidth % 16) != 0) throw new ArgumentException("byteWidth must be a multiple of 16", "byteWidth");

            InitializeCore<T>(byteWidth, data);
        }

        protected abstract void InitializeCore<T>(int byteWidth, T data) where T : struct;

        public abstract void GetData<T>(DeviceContext context, out T data) where T : struct;

        public void SetData<T>(DeviceContext context, T data) where T : struct
        {
            if (Usage == ResourceUsage.Immutable)
                throw new InvalidOperationException("Data can not be set from CPU.");

            var gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                var sourcePointer = gcHandle.AddrOfPinnedObject();
                var sizeInBytes = Marshal.SizeOf(typeof(T));

                unsafe
                {
                    if (Usage == ResourceUsage.Default)
                    {
                        context.UpdateSubresource(this, 0, null, sourcePointer, sizeInBytes, 0);
                    }
                    else
                    {
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
            }
            finally
            {
                gcHandle.Free();
            }
        }
    }
}
