#region Using

using System;

#endregion

namespace Libra.Content
{
    [ContentTypeReader]
    public sealed class RectangleReader : ContentTypeReader<Rectangle>
    {
        protected internal override Rectangle Read(ContentReader input, Rectangle existingInstance)
        {
            return new Rectangle(input.ReadInt32(), input.ReadInt32(), input.ReadInt32(), input.ReadInt32());
        }
    }
}
