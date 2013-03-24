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

        public abstract void SetTexture(string value);

        public abstract void SetDiffuseColor(object value);

        public abstract void SetEmissiveColor(object value);

        public abstract void SetSpecularColor(object value);

        public abstract void SetSpecularPower(float value);

        public abstract void SetAlpha(float value);

        public abstract void SetVertexColorEnabled(bool value);
    }
}
