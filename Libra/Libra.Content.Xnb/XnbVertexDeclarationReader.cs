#region Using

using System;
using Libra.Graphics;

#endregion

namespace Libra.Content.Xnb
{
    [XnbTypeReader("Microsoft.Xna.Framework.Content.VertexDeclarationReader, Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553")]
    public sealed class XnbVertexDeclarationReader : XnbTypeReader<XnbVertexDeclaration>
    {
        protected internal override XnbVertexDeclaration Read(XnbReader input, XnbVertexDeclaration existingInstance)
        {
            var result = new XnbVertexDeclaration();

            // Vertex stride
            result.VertexStride = (int) input.ReadUInt32();

            // Element count
            var elementCount = input.ReadUInt32();

            var elements = new XnbVertexElement[elementCount];

            for (int i = 0; i < elementCount; i++)
            {
                var element = new XnbVertexElement();

                // Offset
                element.Offset = (int) input.ReadUInt32();

                // Element format
                element.VertexElementFormat = (XnbVertexElementFormat) input.ReadInt32();

                // Element usage
                element.VertexElementUsage = (XnbVertexElementUsage) input.ReadInt32();

                // Usage index
                element.UsageIndex = (int) input.ReadUInt32();

                elements[i] = element;
            }

            result.Elements = elements;

            return result;
        }
    }
}
