#region Using

using System;
using System.Collections.Generic;
using Felis.Xnb;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class SpriteFontBuilder : SpriteFontBuilderBase<SpriteFont>
    {
        SpriteFont instance;

        protected override void SetTexture(object value)
        {
            instance.Texture = value as Texture2D;
        }

        protected override void SetGlyphs(object value)
        {
            instance.Glyphs = value as List<Rectangle>;
        }

        protected override void SetCropping(object value)
        {
            instance.Cropping = value as List<Rectangle>;
        }

        protected override void SetCharacterMap(object value)
        {
            instance.Characters = value as List<char>;
        }

        protected override void SetVerticalLineSpacing(int value)
        {
            instance.VerticalLineSpacing = value;
        }

        protected override void SetHorizontalSpacing(float value)
        {
            instance.HorizontalSpacing = value;
        }

        protected override void SetKering(object value)
        {
            instance.Kerning = value as List<Vector3>;
        }

        protected override void SetDefaultCharacter(object value)
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

        protected override void Begin(object deviceContext)
        {
            instance = new SpriteFont();
        }

        protected override object End()
        {
            return instance;
        }
    }
}
