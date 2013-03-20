#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public sealed class ModelMeshPart
    {
        public VertexBuffer VertexBuffer { get; set; }

        public IndexBuffer IndexBuffer { get; set; }

        public int VertexOffset { get; set; }

        public int NumVertices { get; set; }

        public int StartIndex { get; set; }

        public int PrimitiveCount { get; set; }

        public IEffect Effect { get; set; }
    }
}
