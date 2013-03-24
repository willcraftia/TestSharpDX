#region Using

using System;
using Felis.Xnb;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class VertexBufferBuilder : VertexBufferBuilderBase
    {
        VertexBuffer instance;

        public override Type ActualType
        {
            get { return typeof(VertexBuffer); }
        }

        public override void SetVertexDeclaration(object value)
        {
            instance.VertexDeclaration = (VertexDeclaration) value;
        }

        public override void SetVertexCount(uint value)
        {
            instance.VertexCount = (int) value;
        }

        public override uint GetVertexStride()
        {
            return (uint) instance.VertexDeclaration.VertexStride;
        }

        public override void SetVertexData(byte[] value)
        {
            instance.VertexData = value;
        }

        public override void Begin()
        {
            instance = new VertexBuffer();
        }

        public override object End()
        {
            return instance;
        }
    }
}
