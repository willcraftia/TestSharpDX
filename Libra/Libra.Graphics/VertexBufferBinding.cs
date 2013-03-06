#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public struct VertexBufferBinding
    {
        public VertexBuffer VertexBuffer;

        public int Offset;

        public VertexBufferBinding(VertexBuffer vertexBuffer, int offset)
        {
            VertexBuffer = vertexBuffer;
            Offset = offset;
        }
    }
}
