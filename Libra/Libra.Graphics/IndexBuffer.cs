#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class IndexBuffer : Resource
    {
        public IndexFormat Format { get; set; }

        public int IndexCount { get; private set; }

        protected IndexBuffer()
        {
            Format = IndexFormat.SixteenBits;
        }

        public void Initialize(int indexCount)
        {
            if (indexCount < 1) throw new ArgumentOutOfRangeException("indexCount");

            if (Usage == ResourceUsage.Immutable)
                throw new InvalidOperationException("Usage must be not immutable.");

            IndexCount = indexCount;

            InitializeCore();
        }

        public void Initialize<T>(T[] data) where T : struct
        {
            if (data == null) throw new ArgumentNullException("data");
            if (data.Length == 0) throw new ArgumentException("Data must be not empty.", "data");

            IndexCount = data.Length;

            InitializeCore(data);
        }

        protected abstract void InitializeCore();

        protected abstract void InitializeCore<T>(T[] data) where T : struct;

        public void GetData<T>(DeviceContext context, T[] data, int startIndex, int elementCount) where T : struct
        {
            context.GetData(this, 0, data, startIndex, elementCount);
        }

        public void GetData<T>(DeviceContext context, T[] data) where T : struct
        {
            GetData(context, data, 0, data.Length);
        }

        public void SetData<T>(DeviceContext context, T[] data, int startIndex, int elementCount) where T : struct
        {
            context.SetData(this, 0, data, startIndex, elementCount);
        }

        public void SetData<T>(DeviceContext context, params T[] data) where T : struct
        {
            SetData(context, data, 0, data.Length);
        }
    }
}
