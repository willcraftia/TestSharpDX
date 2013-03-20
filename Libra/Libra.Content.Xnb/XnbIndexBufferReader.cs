#region Using

using System;
using Libra.Graphics;

#endregion

namespace Libra.Content.Xnb
{
    [XnbTypeReader("Microsoft.Xna.Framework.Content.IndexBufferReader, Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553")]
    public sealed class XnbIndexBufferReader : XnbTypeReader<IndexBuffer>
    {
        protected internal override IndexBuffer Read(XnbReader input, IndexBuffer existingInstance)
        {
            var result = input.Device.CreateIndexBuffer();

            // Is 16 bit
            var isSixteenBits = input.ReadBoolean();
            result.Format = (isSixteenBits) ? IndexFormat.SixteenBits : IndexFormat.ThirtyTwoBits;

            // Data size
            var dataSize = (int) input.ReadUInt32();

            // Index data
            var indexData = input.ReadBytes(dataSize);

            var indexCount = dataSize;
            if (isSixteenBits)
            {
                indexCount /= 2;
            }
            else
            {
                indexCount /= 4;
            }

            result.Initialize(indexData, indexCount);

            return result;
        }
    }
}
