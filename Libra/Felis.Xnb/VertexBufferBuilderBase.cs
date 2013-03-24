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

        public abstract void SetVertexDeclaration(object value);

        public abstract void SetVertexCount(uint value);

        public abstract uint GetVertexStride();

        public abstract void SetVertexData(byte[] value);
    }
}
