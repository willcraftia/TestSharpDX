#region Using

using System;

#endregion

namespace Libra.Content.Pipeline.Compiler
{
    [ContentTypeWriter]
    public sealed class CharWriter : ContentTypeWriter<char>
    {
        protected internal override void Write(ContentWriter output, char value)
        {
            output.Write(value);
        }
    }
}
