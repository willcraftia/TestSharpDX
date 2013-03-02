#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public interface ISamplerStateCollection
    {
        SamplerState this[int slot] { get; set; }
    }
}
