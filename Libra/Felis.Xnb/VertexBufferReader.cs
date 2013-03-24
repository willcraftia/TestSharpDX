#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public sealed class VertexBufferReader : TypeReader
    {
        VertexBufferBuilderBase builder;

        TypeReader vertexDeclarationReader;

        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.Graphics.VertexBuffer"; }
        }

        public override string ReaderName
        {
            get { return "Microsoft.Xna.Framework.Content.VertexBufferReader"; }
        }

        protected internal override void Initialize(TypeReaderManager manager)
        {
            builder = manager.GetTypeBuilderByTargetType(TargetType) as VertexBufferBuilderBase;
            
            vertexDeclarationReader = new VertexDeclarationReader();
            vertexDeclarationReader.Initialize(manager);

            base.Initialize(manager);
        }

        protected internal override object Read(ContentReader input)
        {
            builder.Begin();

            // Vertex declaration
            builder.SetVertexDeclaration(vertexDeclarationReader.Read(input));

            // Vertex count
            var vertexCount = input.ReadUInt32();
            builder.SetVertexCount(vertexCount);

            int vertexLength = (int) (vertexCount * builder.GetVertexStride());

            // Vertex data
            builder.SetVertexData(input.ReadBytes(vertexLength));

            return builder.End();
        }
    }
}
