#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public sealed class CharReader : TypeReader
    {
        public override string TargetType
        {
            get { return "System.Char"; }
        }

        public override string ReaderName
        {
            get { return "Microsoft.Xna.Framework.Content.CharReader"; }
        }

        protected internal override object Read(ContentReader input)
        {
            return input.ReadChar();
        }
    }
}
