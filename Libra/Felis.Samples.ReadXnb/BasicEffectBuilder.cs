#region Using

using System;
using Felis.Xnb;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class BasicEffectBuilder : BasicEffectBuilderBase<BasicEffect>
    {
        Device device;

        BasicEffect instance;

        protected override void SetTexture(string value)
        {
            instance.Texture = value;
        }

        protected override void SetDiffuseColor(object value)
        {
            instance.DiffuseColor = (Vector3) value;
        }

        protected override void SetEmissiveColor(object value)
        {
            instance.EmissiveColor = (Vector3) value;
        }

        protected override void SetSpecularColor(object value)
        {
            instance.SpecularColor = (Vector3) value;
        }

        protected override void SetSpecularPower(float value)
        {
            instance.SpecularPower = value;
        }

        protected override void SetAlpha(float value)
        {
            instance.Alpha = value;
        }

        protected override void SetVertexColorEnabled(bool value)
        {
            instance.VertexColorEnabled = value;
        }

        protected override void Initialize(ContentManager contentManager)
        {
            device = contentManager.Device as Device;

            base.Initialize(contentManager);
        }

        protected override void Begin(object deviceContext)
        {
            instance = new BasicEffect(device);
        }

        protected override object End()
        {
            return instance;
        }
    }
}
