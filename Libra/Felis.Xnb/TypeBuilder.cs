#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public abstract class TypeBuilder
    {
        public abstract string TargetType { get; }

        public abstract Type ActualType { get; }

        internal bool Initialized { get; private set; }

        protected TypeBuilder() { }

        protected internal virtual void Initialize(ContentManager contentManager)
        {
            Initialized = true;
        }

        protected internal abstract void Begin(object deviceContext);

        protected internal abstract object End();
    }
}
