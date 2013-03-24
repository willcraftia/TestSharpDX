#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public abstract class IndexBufferBuilderBase : TypeBuilder
    {
        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.Graphics.IndexBuffer"; }
        }

        protected IndexBufferBuilderBase() { }

        protected internal abstract void SetIsSixteenBits(bool value);

        protected internal abstract void SetDataSize(uint value);

        protected internal abstract void SetIndexData(byte[] value);
    }

    public abstract class IndexBufferBuilderBase<T> : IndexBufferBuilderBase
    {
        public override Type ActualType
        {
            get { return typeof(T); }
        }

        protected IndexBufferBuilderBase() { }
    }
}
