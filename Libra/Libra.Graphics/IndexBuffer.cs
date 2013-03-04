#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class IndexBuffer : Buffer
    {
        public IndexFormat Format { get; set; }

        protected IndexBuffer()
        {
            Format = IndexFormat.SixteenBits;
        }

        public abstract void Initialize();

        public abstract void Initialize<T>(T[] data) where T : struct;
    }
}
