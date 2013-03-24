#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public abstract class BasicEffectBuilderBase : TypeBuilder
    {
        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.Graphics.BasicEffect"; }
        }

        protected BasicEffectBuilderBase() { }

        protected internal abstract void SetTexture(string value);

        protected internal abstract void SetDiffuseColor(object value);

        protected internal abstract void SetEmissiveColor(object value);

        protected internal abstract void SetSpecularColor(object value);

        protected internal abstract void SetSpecularPower(float value);

        protected internal abstract void SetAlpha(float value);

        protected internal abstract void SetVertexColorEnabled(bool value);
    }

    public abstract class BasicEffectBuilderBase<T> : BasicEffectBuilderBase
    {
        public override Type ActualType
        {
            get { return typeof(T); }
        }

        protected BasicEffectBuilderBase() { }
    }
}
