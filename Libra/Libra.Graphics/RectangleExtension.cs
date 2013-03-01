#region Using

using System;

using SDXRectangle = SharpDX.Rectangle;

#endregion

namespace Libra.Graphics
{
    internal static class RectangleExtension
    {
        internal static SDXRectangle ToSDXRectangle(this Rectangle rectangle)
        {
            return new SDXRectangle(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
        }
    }
}
