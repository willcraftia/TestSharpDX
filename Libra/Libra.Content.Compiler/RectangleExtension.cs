#region Using

using System;

using DrawingRectangle = System.Drawing.Rectangle;

#endregion

namespace Libra.Content.Compiler
{
    internal static class RectangleExtension
    {
        public static DrawingRectangle ToDrawingRectangle(this Rectangle rectangle)
        {
            return new DrawingRectangle(
                rectangle.X,
                rectangle.Y,
                rectangle.Width,
                rectangle.Height);
        }

        public static void FromDrawingRectangle(this Rectangle rectangle, DrawingRectangle drawingRectangle)
        {
            rectangle.X = drawingRectangle.X;
            rectangle.Y = drawingRectangle.Y;
            rectangle.Width = drawingRectangle.Width;
            rectangle.Height = drawingRectangle.Height;
        }
    }
}
