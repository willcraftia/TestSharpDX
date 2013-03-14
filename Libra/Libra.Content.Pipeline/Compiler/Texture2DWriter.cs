#region Using

using System;
using Libra.Graphics;

#endregion

namespace Libra.Content.Pipeline.Compiler
{
    public sealed class Texture2DWriter : ContentTypeWriter<Texture2DContent>
    {
        // Int32:   Surface format (DXGI format value)
        // UInt32:  Width
        // UInt32:  Height
        // UInt32:  Mip count
        // Repeat<mip count>
        // {
        //      UInt32:             Data size
        //      Byte[data size]:    Image data
        // }

        protected internal override void Write(ContentWriter writer, Texture2DContent value)
        {
            if (value.Mipmaps == null) throw new InvalidOperationException("Mipmaps is null.");
            if (value.Mipmaps.Count == 0) throw new InvalidOperationException("Mipmaps is empty.");

            var primeTexture = value.Mipmaps[0];
            if (primeTexture == null) throw new InvalidOperationException("Mipmaps[0] is null.");

            SurfaceFormat format;
            if (!primeTexture.TryGetFormat(out format))
                throw new InvalidOperationException("Unknown surface format.");

            // XNB では専用の値を各フォーマットに割り当てているが、
            // ここでは DXGI Format の値をそのまま設定する。
            writer.Write((int) format);

            writer.Write((uint) primeTexture.Width);
            writer.Write((uint) primeTexture.Height);

            int mipCount = value.Mipmaps.Count;
            writer.Write((uint) mipCount);

            for (int i = 0; i < mipCount; i++)
            {
                var data = value.Mipmaps[i].GetPixelData();
                var dataSize = data.Length;

                writer.Write((uint) dataSize);
                writer.Write(data);
            }
        }
    }
}
