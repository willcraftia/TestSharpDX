#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public interface IShaderResourceViewCollection
    {
        IShaderResourceView this[int slot] { get; }
    }
}
