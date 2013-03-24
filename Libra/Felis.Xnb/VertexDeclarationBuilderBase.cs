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

        public abstract void SetVertexStride(uint value);

        public abstract void SetElementCount(uint value);

        public virtual void BeginElements() { }

        public abstract void BeginElement(int index);

        public abstract void SetElementOffset(uint value);

        public abstract void SetElementFormat(int value);

        public abstract void SetElementUsage(int value);

        public abstract void SetElementUsageIndex(uint value);

        public virtual void EndElement() { }

        public virtual void EndElements() { }
    }
}
