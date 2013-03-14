#region Using

using System;
using System.Collections.Generic;
using System.Drawing;
using Libra.Graphics;

#endregion

namespace Libra.Content.Compiler
{
    public sealed class SpriteFontContent
    {
        public Texture2DContent Texture { get; set; }

        public List<SpriteFontGlyph> Glyphs { get; private set; }

        public float LineSpacing { get; set; }

        public char? DefaultCharacter { get; set; }

        public SpriteFontContent()
        {
            Glyphs = new List<SpriteFontGlyph>();
        }
    }
}
