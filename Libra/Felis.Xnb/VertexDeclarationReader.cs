#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public sealed class VertexDeclarationReader : TypeReader
    {
        VertexDeclarationBuilderBase builder;

        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.Graphics.VertexDeclaration"; }
        }

        public override string ReaderName
        {
            get { return "Microsoft.Xna.Framework.Content.VertexDeclarationReader"; }
        }

        protected internal override void Initialize(TypeReaderManager manager)
        {
            builder = manager.GetTypeBuilderByTargetType(TargetType) as VertexDeclarationBuilderBase;

            base.Initialize(manager);
        }

        protected internal override object Read(ContentReader input)
        {
            builder.Begin();

            // Vertex stride
            builder.SetVertexStride(input.ReadUInt32());

            // Element count
            var elementCount = input.ReadUInt32();
            builder.SetElementCount(elementCount);

            builder.BeginElements();

            // Repeat <element count>
            for (int i = 0; i < elementCount; i++)
            {
                builder.BeginElement(i);

                // Offset
                builder.SetElementOffset(input.ReadUInt32());
                // Element format
                builder.SetElementFormat(input.ReadInt32());
                // Element usage
                builder.SetElementUsage(input.ReadInt32());
                // Usage index
                builder.SetElementUsageIndex(input.ReadUInt32());

                builder.EndElement();
            }

            builder.EndElements();

            return builder.End();
        }
    }
}
