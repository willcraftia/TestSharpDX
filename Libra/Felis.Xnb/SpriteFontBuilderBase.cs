#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public abstract class SpriteFontBuilderBase : TypeBuilder
    {
        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.Graphics.SpriteFont"; }
        }

        public abstract void SetTexture(object value);

        public abstract void SetGlyphs(object value);

        public abstract void SetCropping(object value);

        public abstract void SetCharacterMap(object value);

        public abstract void SetVerticalLineSpacing(int value);

        public abstract void SetHorizontalSpacing(float value);

        public abstract void SetKering(object value);

        public abstract void SetDefaultCharacter(object value);
    }
}
