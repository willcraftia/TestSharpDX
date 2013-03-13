#region Using

using System;

#endregion

namespace Libra.Content.Serialization
{
    [Flags]
    public enum FontDescriptionStyle
    {
        Regular     = 0x0,
        Bold        = 0x1,
        Italic      = 0x2,
        Underline   = 0x4,
        Strikeout   = 0x8,
    }
}
