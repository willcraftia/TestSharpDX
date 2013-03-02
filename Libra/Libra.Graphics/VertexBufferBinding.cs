#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public struct VertexBufferBinding
    {
        public IVertexBuffer VertexBuffer;

        public int Stride;

        public int Offset;

        public VertexBufferBinding(IVertexBuffer vertexBuffer, int stride, int offset)
        {
            VertexBuffer = vertexBuffer;
            Stride = stride;
            Offset = offset;
        }
    }
}
