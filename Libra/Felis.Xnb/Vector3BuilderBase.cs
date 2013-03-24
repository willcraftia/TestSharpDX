#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public abstract class Vector3BuilderBase : TypeBuilder
    {
        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.Vector3"; }
        }

        protected Vector3BuilderBase() { }

        protected internal abstract void SetValues(float x, float y, float z);
    }

    public abstract class Vector3BuilderBase<T> : Vector3BuilderBase
    {
        public override Type ActualType
        {
            get { return typeof(T); }
        }

        protected Vector3BuilderBase() { }
    }
}
