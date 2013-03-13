#region Using

using System;

using DrawingColor = System.Drawing.Color;

#endregion

namespace Libra.Content.Compiler
{
    public static class ColorExtension
    {
        public static DrawingColor ToDrawingColor(this Color color)
        {
            return DrawingColor.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
