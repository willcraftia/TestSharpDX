#region Using

using System;
using System.Collections.Generic;
using Felis.Xnb;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class SpriteFontBuilder : SpriteFontBuilderBase
    {
        SpriteFont instance;

        public override Type ActualType
        {
            get { return typeof(SpriteFont); }
        }

        public override void SetTexture(object value)
        {
            instance.Texture = value as Texture2D;
        }

        public override void SetGlyphs(object value)
        {
            instance.Glyphs = value as List<Rectangle>;
        }

        public override void SetCropping(object value)
        {
            instance.Cropping = value as List<Rectangle>;
        }

        public override void SetCharacterMap(object value)
        {
            instance.Characters = value as List<char>;
        }

        public override void SetVerticalLineSpacing(int value)
        {
            instance.VerticalLineSpacing = value;
        }

        public override void SetHorizontalSpacing(float value)
        {
            instance.HorizontalSpacing = value;
        }

        public override void SetKering(object value)
        {
            instance.Kerning = value as List<Vector3>;
        }

        public override void SetDefaultCharacter(object value)
        {
            if (value == null)
            {
                instance.DefaultCharacter = null;
            }
            else
            {
                instance.DefaultCharacter = value as char?;
            }
        }

        public override void Begin()
        {
            instance = new SpriteFont();
        }

        public override object End()
        {
            return instance;
        }
    }
}
