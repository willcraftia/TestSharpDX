#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class VertexBuffer : Buffer
    {
        protected VertexBuffer() { }

        public abstract void Initialize();

        public abstract void Initialize<T>(T[] data) where T : struct;
    }
}
