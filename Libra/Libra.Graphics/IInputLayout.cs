#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public interface IInputLayout : IDisposable
    {
        int InputStride { get; }
    }
}
