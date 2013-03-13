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
        public BitmapContent Bitmap { get; set; }

        public bool PremultiplyAlpha { get; set; }

        public List<SpriteFontGlyph> Glyphs { get; private set; }

        public float LineSpacing { get; set; }

        public char? DefaultCharacter { get; set; }

        public SurfaceFormat SurfaceFormat { get; set; }

        public SpriteFontContent()
        {
            Glyphs = new List<SpriteFontGlyph>();
        }
    }
}
