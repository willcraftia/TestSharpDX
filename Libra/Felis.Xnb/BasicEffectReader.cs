#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public sealed class BasicEffectReader : TypeReader
    {
        BasicEffectBuilderBase builder;

        Vector3Reader vector3Reader;

        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.Graphics.BasicEffect"; }
        }

        public override string ReaderName
        {
            get { return "Microsoft.Xna.Framework.Content.BasicEffectReader"; }
        }

        protected internal override void Initialize(TypeReaderManager manager)
        {
            builder = manager.GetTypeBuilderByTargetType(TargetType) as BasicEffectBuilderBase;

            vector3Reader = new Vector3Reader();
            vector3Reader.Initialize(manager);

            base.Initialize(manager);
        }

        protected internal override object Read(XnbReader input)
        {
            builder.Begin();

            // Texture
            builder.SetTexture(input.ReadString());

            // Diffuse color
            builder.SetDiffuseColor(vector3Reader.Read(input));

            // Emissive color
            builder.SetEmissiveColor(vector3Reader.Read(input));

            // Specular color
            builder.SetSpecularColor(vector3Reader.Read(input));

            // Specular power
            builder.SetSpecularPower(input.ReadSingle());

            // Alpha
            builder.SetAlpha(input.ReadSingle());

            // Vertex color enabled
            builder.SetVertexColorEnabled(input.ReadBoolean());

            return builder.End();
        }
    }
}
