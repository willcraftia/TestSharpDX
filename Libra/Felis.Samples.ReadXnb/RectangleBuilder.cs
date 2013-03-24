#region Using

using System;
using Felis.Xnb;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class RectangleBuilder : RectangleBuilderBase
    {
        Rectangle instance;

        public override Type ActualType
        {
            get { return typeof(Rectangle); }
        }

        public override void SetValues(int x, int y, int width, int height)
        {
            instance.X = x;
            instance.Y = y;
            instance.Width = width;
            instance.Height = height;
        }

        public override void Begin()
        {
            instance = new Rectangle();

            base.Begin();
        }

        public override object End()
        {
            return instance;
        }
    }
}
