#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public abstract class TypeBuilder
    {
        public abstract string TargetType { get; }

        public abstract Type ActualType { get; }

        protected TypeBuilder() { }

        public virtual void Begin() { }

        public abstract object End();
    }
}
