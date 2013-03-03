#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class ConstantBuffer : Buffer
    {
        protected ConstantBuffer() { }

        public abstract void Initialize();

        public abstract void Initialize<T>() where T : struct;
    }
}
