#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public interface IStage
    {
        IDeviceContext DeviceContext { get; }
    }
}
