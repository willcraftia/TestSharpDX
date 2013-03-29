#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public struct VertexDeclarationBinding
    {
        public VertexDeclaration VertexDeclaration;

        public int Slot;

        public VertexDeclarationBinding(VertexDeclaration vertexDeclaration, int slot = 0)
        {
            VertexDeclaration = vertexDeclaration;
            Slot = slot;
        }
    }
}
