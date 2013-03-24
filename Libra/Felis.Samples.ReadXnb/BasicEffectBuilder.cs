#region Using

using System;
using Felis.Xnb;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class BasicEffectBuilder : BasicEffectBuilderBase
    {
        BasicEffect instance;

        public override Type ActualType
        {
            get { return typeof(BasicEffect); }
        }

        public override void SetTexture(string value)
        {
            instance.Texture = value;
        }

        public override void SetDiffuseColor(object value)
        {
            instance.DiffuseColor = (Vector3) value;
        }

        public override void SetEmissiveColor(object value)
        {
            instance.EmissiveColor = (Vector3) value;
        }

        public override void SetSpecularColor(object value)
        {
            instance.SpecularColor = (Vector3) value;
        }

        public override void SetSpecularPower(float value)
        {
            instance.SpecularPower = value;
        }

        public override void SetAlpha(float value)
        {
            instance.Alpha = value;
        }

        public override void SetVertexColorEnabled(bool value)
        {
            instance.VertexColorEnabled = value;
        }

        public override void Begin()
        {
            instance = new BasicEffect();
        }

        public override object End()
        {
            return instance;
        }
    }
}
