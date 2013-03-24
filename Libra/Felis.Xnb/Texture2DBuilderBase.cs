#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public abstract class Texture2DBuilderBase : TypeBuilder
    {
        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.Graphics.Texture2D"; }
        }

        public abstract void SetSurfaceFormat(int value);

        public abstract void SetWidth(uint value);

        public abstract void SetHeight(uint value);

        public abstract void SetMipCount(uint value);

        public virtual void BeginMips() { }

        public abstract void BeginMip(int index);

        public abstract void SetMipDataSize(uint value);

        public abstract void SetMipImageData(byte[] value);

        public virtual void EndMip() { }

        public virtual void EndMips() { }
    }
}
