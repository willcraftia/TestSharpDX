#region Using

using System;

using DrawingBitmap = System.Drawing.Bitmap;

#endregion

namespace Libra.Content.Compiler
{
    public sealed class SpriteFontGlyph
    {
        public char Character;

        public Rectangle Cropping;

        // TODO
        //
        // 以下は、恐らく、カーニング情報だと思われるが、
        // XNA の仕組みとは異なる可能性が高い。

        public float XOffset;

        public float YOffset;

        public float XAdvance;
    }
}
