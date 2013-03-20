#region Using

using System;

#endregion

namespace Libra.Content.Xnb
{
    public sealed class XnbVertexDeclaration
    {
        public int VertexStride { get; set; }

        public XnbVertexElement[] Elements { get; set; }
    }
}
