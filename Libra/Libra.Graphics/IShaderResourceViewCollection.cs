#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public interface IShaderResourceViewCollection
    {
        ShaderResourceView this[int slot] { get; }
    }
}
