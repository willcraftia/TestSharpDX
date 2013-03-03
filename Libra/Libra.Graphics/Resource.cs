#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class Resource : IDisposable
    {
        public string Name { get; set; }

        public ResourceUsage Usage { get; set; }

        protected Resource() { }

        public abstract void GetData<T>(IDeviceContext context, int level, T[] data, int startIndex, int elementCount) where T : struct;

        public void GetData<T>(IDeviceContext context, int level, T[] data) where T : struct
        {
            GetData(context, level, data, 0, data.Length);
        }

        public void GetData<T>(IDeviceContext context, T[] data, int startIndex, int elementCount) where T : struct
        {
            GetData(context, 0, data, startIndex, elementCount);
        }

        public void GetData<T>(IDeviceContext context, T[] data) where T : struct
        {
            GetData(context, 0, data, 0, data.Length);
        }

        public abstract void SetData<T>(IDeviceContext context, T[] data, int startIndex, int elementCount) where T : struct;

        public void SetData<T>(IDeviceContext context, params T[] data) where T : struct
        {
            SetData(context, data, 0, data.Length);
        }

        #region ToString

        public override string ToString()
        {
            if (Name != null)
                return "{Name:" + Name + "}";

            return base.ToString();
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; private set; }

        ~Resource()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void DisposeOverride(bool disposing) { }

        void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            DisposeOverride(disposing);

            IsDisposed = true;
        }

        #endregion
    }
}
