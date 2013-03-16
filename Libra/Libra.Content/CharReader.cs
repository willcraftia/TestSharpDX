#region Using

using System;

#endregion

namespace Libra.Content
{
    [ContentTypeReader]
    public sealed class CharReader : ContentTypeReader<char>
    {
        protected internal override char Read(ContentReader input, char existingInstance)
        {
            return input.ReadChar();
        }
    }
}
