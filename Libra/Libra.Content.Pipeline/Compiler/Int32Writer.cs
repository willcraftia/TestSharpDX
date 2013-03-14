#region Using

using System;

#endregion

namespace Libra.Content.Pipeline.Compiler
{
    [ContentTypeWriter]
    public sealed class Int32Writer : ContentTypeWriter<int>
    {
        protected internal override void Write(ContentWriter output, int value)
        {
            output.Write(value);
        }
    }
}
