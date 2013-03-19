#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public sealed class ModelMeshPart
    {
        public VertexBuffer VertexBuffer { get; private set; }

        public IndexBuffer IndexBuffer { get; private set; }

        public ModelMeshPart(VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
        {
            if (vertexBuffer == null) throw new ArgumentNullException("vertexBuffer");
            if (indexBuffer == null) throw new ArgumentNullException("indexBuffer");

            VertexBuffer = vertexBuffer;
            IndexBuffer = indexBuffer;
        }
    }
}
