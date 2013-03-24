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

        public abstract void SetValues(int x, int y, int width, int height);
    }
}
