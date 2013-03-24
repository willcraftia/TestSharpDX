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

        protected Texture2DBuilderBase() { }

        protected internal abstract void SetSurfaceFormat(int value);

        protected internal abstract void SetWidth(uint value);

        protected internal abstract void SetHeight(uint value);

        protected internal abstract void SetMipCount(uint value);

        protected internal virtual void BeginMips() { }

        protected internal abstract void BeginMip(int index);

        protected internal abstract void SetMipDataSize(uint value);

        protected internal abstract void SetMipImageData(byte[] value);

        protected internal virtual void EndMip() { }

        protected internal virtual void EndMips() { }
    }

    public abstract class Texture2DBuilderBase<T> : Texture2DBuilderBase
    {
        public override Type ActualType
        {
            get { return typeof(T); }
        }

        protected Texture2DBuilderBase() { }
    }
}
