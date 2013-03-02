#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public interface IResource : IDisposable
    {
        event EventHandler Disposing;

        ResourceUsage Usage { get; }

        string Name { get; set; }

        object Tag { get; set; }

        void GetData<T>(IDeviceContext context, int level, T[] data, int startIndex, int elementCount) where T : struct;

        void SetData<T>(IDeviceContext context, params T[] data) where T : struct;

        void SetData<T>(IDeviceContext context, T[] data, int startIndex, int elementCount) where T : struct;
    }
}
