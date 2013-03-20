#region Using

using System;

#endregion

namespace Libra.Content.Xnb
{
    public struct XnbVertexElement
    {
        public int Offset { get; set; }

        public XnbVertexElementFormat VertexElementFormat { get; set; }

        public XnbVertexElementUsage VertexElementUsage { get; set; }

        public int UsageIndex { get; set; }
    }
}
