#region Using

using System;
using System.IO;
using Libra.Graphics;

#endregion

namespace Libra.Content.Xnb
{
    [XnbTypeReader("Microsoft.Xna.Framework.Content.BasicEffectReader, Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553")]
    public sealed class XnbBasicEffectReader : XnbTypeReader<BasicEffect>
    {
        XnbTypeReader externalReferenceReader;

        protected internal override void Initialize(XnbTypeReaderManager manager)
        {
            externalReferenceReader = manager.GetTypeReader(typeof(XnbExternalReference));

            base.Initialize(manager);
        }

        protected internal override BasicEffect Read(XnbReader input, BasicEffect existingInstance)
        {
            var result = new BasicEffect(input.Manager.Device);

            // Texture
            var textureReference = (XnbExternalReference) externalReferenceReader.Read(input, new XnbExternalReference());

            if (textureReference.AssetName != null)
            {
                // テクスチャの実体の取得。
                var texture = input.Manager.Load<Texture2D>(textureReference.AssetName);
                var textureView = input.Manager.Device.CreateShaderResourceView();
                textureView.Initialize(texture);

                result.Texture = textureView;
                result.TextureEnabled = true;
            }

            // Diffuse color
            result.DiffuseColor = input.ReadVector3();
            
            // Emissive color
            result.EmissiveColor = input.ReadVector3();

            // Specular color
            result.SpecularColor = input.ReadVector3();

            // Specular power
            result.SpecularPower = input.ReadSingle();

            // Alpha
            result.Alpha = input.ReadSingle();

            // Vertex color enabled
            result.VertexColorEnabled = input.ReadBoolean();

            return result;
        }
    }
}
