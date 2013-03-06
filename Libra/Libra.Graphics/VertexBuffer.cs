#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class VertexBuffer : Resource
    {
        public int ByteWidth { get; set; }

        protected VertexBuffer() { }

        public abstract void Initialize();

        public abstract void Initialize<T>(T[] data) where T : struct;

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
    }
}
