#region Using

using System;
using Libra.Graphics;

#endregion

namespace Libra.Content.Xnb
{
    [XnbTypeReader("Microsoft.Xna.Framework.Content.Texture2DReader, Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553")]
    public sealed class XnbTexture2DReader : XnbTypeReader<Texture2D>
    {
        protected internal override Texture2D Read(XnbReader input, Texture2D existingInstance)
        {
            var result = input.Manager.Device.CreateTexture2D();

            // Surface format
            var surfaceFormat = input.ReadInt32();
            result.Format = XnbSurfaceFormatHelper.ToSurfaceFormat(surfaceFormat);

            // Witdh
            result.Width = (int) input.ReadUInt32();

            // Height
            result.Height = (int) input.ReadUInt32();

            // Mip count
            result.MipLevels = (int) input.ReadUInt32();

            result.Initialize();

            // Repeat <mip count>
            for (int i = 0; i < result.MipLevels; i++)
            {
                // Data size
                var dataSize = (int) input.ReadUInt32();

                // Image data
                var imageData = input.ReadBytes(dataSize);

                result.SetData(input.DeviceContext, i, imageData);
            }

            return result;
        }
    }
}
