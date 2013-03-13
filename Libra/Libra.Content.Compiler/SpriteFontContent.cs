#region Using

using System;
using System.Collections.Generic;
using System.Drawing;

#endregion

namespace Libra.Content.Compiler
{
    public sealed class SpriteFontContent
    {
        public List<SpriteFontGlyph> Glyphs { get; private set; }

        public float LineSpacing { get; set; }

        public char? DefaultCharacter { get; set; }

        public Bitmap Bitmap { get; set; }

        public bool PremultiplyAlpha { get; set; }

        public SpriteFontContent()
        {
            Glyphs = new List<SpriteFontGlyph>();
        }
    }
}
