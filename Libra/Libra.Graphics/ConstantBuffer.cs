#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class ConstantBuffer : Resource
    {
        protected ConstantBuffer() { }

        public abstract void Initialize();

        public abstract void Initialize<T>() where T : struct;

        public void GetData<T>(DeviceContext context, T[] data) where T : struct
        {
            context.GetData(this, 0, data, 0, data.Length);
        }

        public void SetData<T>(DeviceContext context, T data) where T : struct
        {
            context.SetData(this, 0, data);
        }
    }
}
