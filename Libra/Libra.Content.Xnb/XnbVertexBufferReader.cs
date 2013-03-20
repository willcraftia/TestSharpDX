#region Using

using System;
using Libra.Graphics;

#endregion

namespace Libra.Content.Xnb
{
    [XnbTypeReader("Microsoft.Xna.Framework.Content.VertexBufferReader, Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553")]
    public sealed class XnbVertexBufferReader : XnbTypeReader<VertexBuffer>
    {
        XnbTypeReader vertexDeclarationReader;

        protected internal override void Initialize(XnbTypeReaderManager manager)
        {
            vertexDeclarationReader = manager.GetTypeReader(typeof(XnbVertexDeclaration));

            base.Initialize(manager);
        }

        protected internal override VertexBuffer Read(XnbReader input, VertexBuffer existingInstance)
        {
            // Vertex declaration
            // ReadObject 経由ではなく直接読み込みである点に注意。
            var vertexDeclaration = vertexDeclarationReader.Read(input, null) as XnbVertexDeclaration;

            // Vertex count
            var vertexCount = (int) input.ReadUInt32();

            var vertexLength = vertexCount * vertexDeclaration.VertexStride;

            // Vertex data
            var vertexData = input.ReadBytes(vertexLength);

            var result = input.Manager.Device.CreateVertexBuffer();
            
            result.Usage = ResourceUsage.Default;
            result.Initialize(vertexData, vertexDeclaration.VertexStride);

            return result;
        }
    }
}
