#region Using

using System;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class ModelMeshPart
    {
        public int VertexOffset;

        public int NumVertices;

        public int StartIndex;

        public int PrimitiveCount;

        public object Tag;

        public VertexBuffer VertexBuffer;

        public IndexBuffer IndexBuffer;

        public Effect Effect;
    }
}
