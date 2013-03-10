#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class VertexBuffer : Resource
    {
        public int VertexStride { get; private set; }

        public int VertexCount { get; private set; }

        protected VertexBuffer() { }

        public void Initialize(int vertexStride, int vertexCount)
        {
            if (vertexStride < 1) throw new ArgumentOutOfRangeException("vertexStride");
            if (vertexCount < 1) throw new ArgumentOutOfRangeException("vertexCount");

            if (Usage == ResourceUsage.Immutable)
                throw new InvalidOperationException("Usage must be not immutable.");

            VertexStride = vertexStride;
            VertexCount = vertexCount;

            InitializeCore();
        }

        public void Initialize<T>(T[] data) where T : struct
        {
            if (data == null) throw new ArgumentNullException("data");
            if (data.Length == 0) throw new ArgumentException("Data must be not empty.", "data");

            VertexCount = data.Length;

            // 少し歪になるが、ジェネリクス対応の sizeof は現状 SharpDX に頼らざるを得ないため、
            // 抽象メソッド実装側で VertexStride を返却してもらう。
            VertexStride = InitializeCore(data);
        }

        protected abstract void InitializeCore();

        protected abstract int InitializeCore<T>(T[] data) where T : struct;

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
            context.SetData(this, data, startIndex, elementCount);
        }

        public void SetData<T>(DeviceContext context, params T[] data) where T : struct
        {
            SetData(context, data, 0, data.Length);
        }

        public void SetData<T>(DeviceContext context, T[] data, int sourceIndex, int elementCount,
            int destinationIndex, SetDataOptions options = SetDataOptions.None) where T : struct
        {
            context.SetData(this, data, sourceIndex, elementCount, destinationIndex, options);
        }
    }
}
