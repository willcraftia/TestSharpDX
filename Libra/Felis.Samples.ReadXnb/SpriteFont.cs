#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class SpriteFont
    {
        public Texture2D Texture;

        public List<Rectangle> Glyphs;

        public List<Rectangle> Cropping;

        public List<char> Characters;

        public int VerticalLineSpacing;

        public float HorizontalSpacing;

        public List<Vector3> Kerning;

        public char? DefaultCharacter;
    }
}
