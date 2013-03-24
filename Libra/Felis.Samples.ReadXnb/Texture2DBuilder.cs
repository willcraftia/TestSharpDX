#region Using

using System;
using Felis.Xnb;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class Texture2DBuilder : Texture2DBuilderBase
    {
        Texture2D instance;

        Mip currentMip;

        public override Type ActualType
        {
            get { return typeof(Texture2D); }
        }

        public override void SetSurfaceFormat(int value)
        {
            instance.SurfaceFormat = value;
        }

        public override void SetWidth(uint value)
        {
            instance.Width = (int) value;
        }

        public override void SetHeight(uint value)
        {
            instance.Height = (int) value;
        }

        public override void SetMipCount(uint value)
        {
            instance.Mips = new Mip[value];
        }

        public override void BeginMip(int index)
        {
            instance.Mips[index] = new Mip();
            currentMip = instance.Mips[index];
        }

        public override void SetMipDataSize(uint value)
        {
            currentMip.DataSize = (int) value;
        }

        public override void SetMipImageData(byte[] value)
        {
            currentMip.ImageData = value;
        }

        public override void Begin()
        {
            instance = new Texture2D();
        }

        public override object End()
        {
            return instance;
        }
    }
}
