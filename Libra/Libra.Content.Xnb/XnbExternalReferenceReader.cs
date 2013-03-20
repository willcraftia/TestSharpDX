#region Using

using System;

#endregion

namespace Libra.Content.Xnb
{
    // TODO
    // XNB 内でのクラス名文字列が完全修飾名か否かが不明。
    [XnbTypeReader("Microsoft.Xna.Framework.Content.ExternalReferenceReader")]
    public sealed class XnbExternalReferenceReader : XnbTypeReader<XnbExternalReference>
    {
        protected internal override XnbExternalReference Read(XnbReader input, XnbExternalReference existingInstance)
        {
            // Asset name
            var assetName = input.ReadString();

            return new XnbExternalReference { AssetName = assetName };
        }
    }
}
