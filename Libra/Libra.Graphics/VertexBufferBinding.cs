#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public struct VertexBufferBinding
    {
        public VertexBuffer VertexBuffer;

        public int Stride;

        public int Offset;

        public VertexBufferBinding(VertexBuffer vertexBuffer, int stride, int offset)
        {
            VertexBuffer = vertexBuffer;
            Stride = stride;
            Offset = offset;
        }
    }
}
