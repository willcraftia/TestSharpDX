#region Using

using System;
using System.Drawing;

#endregion

// DirectX Tool Kit より幾つかのクラスを移植して合成。

namespace Libra.Content.Compiler
{
    public sealed class SpriteFontGlyph
    {
        public char Character;

        public Bitmap Bitmap;

        public Rectangle Subrect;

        public float XOffset;

        public float YOffset;

        public float XAdvance;

        public SpriteFontGlyph(char character, Bitmap bitmap, Rectangle? subrect = null)
        {
            Character = character;
            Bitmap = bitmap;
            Subrect = subrect.GetValueOrDefault(new Rectangle(0, 0, bitmap.Width, bitmap.Height));
        }
    }
}
