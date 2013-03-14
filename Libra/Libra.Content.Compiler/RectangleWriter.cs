#region Using

using System;

#endregion

namespace Libra.Content.Compiler
{
    public sealed class RectangleWriter : ContentTypeWriter<Rectangle>
    {
        protected internal override void Write(ContentWriter output, Rectangle value)
        {
            output.Write(value.X);
            output.Write(value.Y);
            output.Write(value.Width);
            output.Write(value.Height);
        }
    }
}
