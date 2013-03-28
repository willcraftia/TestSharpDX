#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public sealed class ModelMeshPart
    {
        public VertexBuffer VertexBuffer { get; set; }

        public IndexBuffer IndexBuffer { get; set; }

        public InputLayout InputLayout { get; set; }

        // XNA に合わせて頂点数単位のオフセットを設定。
        public int VertexOffset { get; set; }

        public int NumVertices { get; set; }

        public int StartIndex { get; set; }

        public int PrimitiveCount { get; set; }

        public IEffect Effect { get; set; }
    }
}
