#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public abstract class BoundingSphereBuilderBase : TypeBuilder
    {
        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.BoundingSphere"; }
        }

        protected BoundingSphereBuilderBase() { }

        protected internal abstract void SetCenter(object value);

        protected internal abstract void SetRadius(float value);
    }

    public abstract class BoundingSphereBuilderBase<T> : BoundingSphereBuilderBase
    {
        public override Type ActualType
        {
            get { return typeof(T); }
        }

        protected BoundingSphereBuilderBase() { }
    }
}
