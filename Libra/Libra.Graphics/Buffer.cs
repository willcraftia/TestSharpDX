#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class Buffer : Resource
    {
        public int ByteWidth { get; set; }
    }
}
