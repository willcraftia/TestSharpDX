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

        protected SpriteFontBuilderBase() { }

        protected internal abstract void SetTexture(object value);

        protected internal abstract void SetGlyphs(object value);

        protected internal abstract void SetCropping(object value);

        protected internal abstract void SetCharacterMap(object value);

        protected internal abstract void SetVerticalLineSpacing(int value);

        protected internal abstract void SetHorizontalSpacing(float value);

        protected internal abstract void SetKering(object value);

        protected internal abstract void SetDefaultCharacter(object value);
    }

    public abstract class SpriteFontBuilderBase<T> : SpriteFontBuilderBase
    {
        public override Type ActualType
        {
            get { return typeof(T); }
        }

        protected SpriteFontBuilderBase() { }
    }
}
