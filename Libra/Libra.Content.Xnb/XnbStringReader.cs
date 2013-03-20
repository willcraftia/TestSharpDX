#region Using

using System;

#endregion

namespace Libra.Content.Xnb
{
    [XnbTypeReader("Microsoft.Xna.Framework.Content.StringReader")]
    public sealed class XnbStringReader : XnbTypeReader<string>
    {
        protected internal override string Read(XnbReader input, string existingInstance)
        {
            return input.ReadString();
        }
    }
}
