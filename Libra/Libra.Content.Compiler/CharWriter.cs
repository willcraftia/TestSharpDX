#region Using

using System;

#endregion

namespace Libra.Content.Compiler
{
    public sealed class CharWriter : ContentTypeWriter<char>
    {
        protected internal override void Write(ContentWriter output, char value)
        {
            output.Write(value);
        }
    }
}
