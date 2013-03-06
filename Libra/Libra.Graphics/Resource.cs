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
