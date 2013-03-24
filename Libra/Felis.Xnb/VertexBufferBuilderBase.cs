#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public abstract class VertexBufferBuilderBase : TypeBuilder
    {
        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.Graphics.VertexBuffer"; }
        }

        protected VertexBufferBuilderBase() { }

        protected internal abstract void SetVertexDeclaration(object value);

        protected internal abstract void SetVertexCount(uint value);

        protected internal abstract uint GetVertexStride();

        protected internal abstract void SetVertexData(byte[] value);
    }

    public abstract class VertexBufferBuilderBase<T> : VertexBufferBuilderBase
    {
        public override Type ActualType
        {
            get { return typeof(T); }
        }

        protected VertexBufferBuilderBase() { }
    }
}
