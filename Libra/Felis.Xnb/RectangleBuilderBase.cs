#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public abstract class RectangleBuilderBase : TypeBuilder
    {
        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.Rectangle"; }
        }

        protected RectangleBuilderBase() { }

        protected internal abstract void SetValues(int x, int y, int width, int height);
    }

    public abstract class RectangleBuilderBase<T> : RectangleBuilderBase
    {
        public override Type ActualType
        {
            get { return typeof(T); }
        }

        protected RectangleBuilderBase() { }
    }
}
