#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public abstract class VertexDeclarationBuilderBase : TypeBuilder
    {
        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.Graphics.VertexDeclaration"; }
        }

        protected VertexDeclarationBuilderBase() { }

        protected internal abstract void SetVertexStride(uint value);

        protected internal abstract void SetElementCount(uint value);

        protected internal virtual void BeginElements() { }

        protected internal abstract void BeginElement(int index);

        protected internal abstract void SetElementOffset(uint value);

        protected internal abstract void SetElementFormat(int value);

        protected internal abstract void SetElementUsage(int value);

        protected internal abstract void SetElementUsageIndex(uint value);

        protected internal virtual void EndElement() { }

        protected internal virtual void EndElements() { }
    }

    public abstract class VertexDeclarationBuilderBase<T> : VertexDeclarationBuilderBase
    {
        public override Type ActualType
        {
            get { return typeof(T); }
        }

        protected VertexDeclarationBuilderBase() { }
    }
}
