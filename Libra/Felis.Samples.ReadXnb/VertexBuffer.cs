#region Using

using System;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class VertexBuffer
    {
        public VertexDeclaration VertexDeclaration;

        public int VertexCount;

        public byte[] VertexData;

        public VertexBuffer(Device device) { }
    }
}
