#region Using

using System;
using Libra.Graphics;

#endregion

namespace Libra.Content.Xnb
{
    [XnbTypeReader("Microsoft.Xna.Framework.Content.VertexDeclarationReader, Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553")]
    public sealed class XnbVertexDeclarationReader : XnbTypeReader<VertexDeclaration>
    {
        protected internal override VertexDeclaration Read(XnbReader input, VertexDeclaration existingInstance)
        {
            // Vertex stride
            // VertexDeclaration 内部で自動決定もできるが、
            // 簡単ではない頂点要素の並びを用いる可能性もあるため、
            // 明示的な値を利用する。
            var vertexStride = (int) input.ReadUInt32();

            // Element count
            var elementCount = input.ReadUInt32();

            var elements = new InputElement[elementCount];

            for (int i = 0; i < elementCount; i++)
            {
                var element = new InputElement();

                // Offset
                // InputElement 内部で自動決定もできるが、
                // 簡単ではない頂点要素の並びを用いる可能性もあるため、
                // 明示的な値を利用する。
                element.AlignedByteOffset = (int) input.ReadUInt32();

                // Element format
                element.Format = XnbVertexElementFormatHelper.ToInputElement(input.ReadInt32());

                // Element usage
                element.SemanticName = XnbVertexElementUsageHelper.ToSemanticsName(input.ReadInt32());

                // Usage index
                element.SemanticIndex = (int) input.ReadUInt32();

                elements[i] = element;
            }

            // slot = 0 固定。
            // また、インスタンス データを扱う事はない。
            return new VertexDeclaration(vertexStride, elements);
        }
    }
}
