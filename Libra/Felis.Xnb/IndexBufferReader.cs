#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public sealed class IndexBufferReader : TypeReader
    {
        IndexBufferBuilderBase builder;

        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.Graphics.IndexBuffer"; }
        }

        public override string ReaderName
        {
            get { return "Microsoft.Xna.Framework.Content.IndexBufferReader"; }
        }

        protected internal override void Initialize(TypeReaderManager manager)
        {
            builder = manager.GetTypeBuilderByTargetType(TargetType) as IndexBufferBuilderBase;

            base.Initialize(manager);
        }
        
        protected internal override object Read(ContentReader input)
        {
            builder.Begin(input.DeviceContext);

            // Is 16 bit
            builder.SetIsSixteenBits(input.ReadBoolean());

            // Data size
            var dataSize = input.ReadUInt32();
            builder.SetDataSize(dataSize);

            // Index data
            builder.SetIndexData(input.ReadBytes((int) dataSize));

            return builder.End();
        }
    }
}
