#region Using

using System;
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
            var result = new BasicEffect(input.Device);

            // Texture
            var textureReference = (XnbExternalReference) externalReferenceReader.Read(input, new XnbExternalReference());
            Console.WriteLine(textureReference);

            // TODO
            // テクスチャのアセットとしてのロードと設定。
            // 恐らく、ContentManager を作るべきなのだろうと思う。

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
