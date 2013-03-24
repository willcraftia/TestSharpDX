#region Using

using System;
using Felis.Xnb;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class Texture2DBuilder : Texture2DBuilderBase<Texture2D>
    {
        Texture2D instance;

        Mip currentMip;

        protected override void SetSurfaceFormat(int value)
        {
            instance.SurfaceFormat = value;
        }

        protected override void SetWidth(uint value)
        {
            instance.Width = (int) value;
        }

        protected override void SetHeight(uint value)
        {
            instance.Height = (int) value;
        }

        protected override void SetMipCount(uint value)
        {
            instance.Mips = new Mip[value];
        }

        protected override void BeginMip(int index)
        {
            instance.Mips[index] = new Mip();
            currentMip = instance.Mips[index];
        }

        protected override void SetMipDataSize(uint value)
        {
            currentMip.DataSize = (int) value;
        }

        protected override void SetMipImageData(byte[] value)
        {
            currentMip.ImageData = value;
        }

        protected override void Begin(object deviceContext)
        {
            instance = new Texture2D();
        }

        protected override object End()
        {
            return instance;
        }
    }
}
