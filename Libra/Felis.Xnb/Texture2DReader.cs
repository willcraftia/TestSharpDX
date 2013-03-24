#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public sealed class Texture2DReader : TypeReader
    {
        Texture2DBuilderBase builder;

        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.Graphics.Texture2D"; }
        }

        public override string ReaderName
        {
            get { return "Microsoft.Xna.Framework.Content.Texture2DReader"; }
        }

        protected internal override void Initialize(TypeReaderManager manager)
        {
            builder = manager.GetTypeBuilderByTargetType(TargetType) as Texture2DBuilderBase;
            
            base.Initialize(manager);
        }

        protected internal override object Read(ContentReader input)
        {
            builder.Begin();

            // Surface format
            builder.SetSurfaceFormat(input.ReadInt32());

            // Witdh
            builder.SetWidth(input.ReadUInt32());

            // Height
            builder.SetHeight(input.ReadUInt32());

            // Mip count
            var mipCount = input.ReadUInt32();
            builder.SetMipCount(mipCount);

            builder.BeginMips();

            // Repeat <mip count>
            for (int i = 0; i < mipCount; i++)
            {
                builder.BeginMip(i);

                // Data size
                var dataSize = input.ReadUInt32();
                builder.SetMipDataSize(dataSize);

                // Image data
                builder.SetMipImageData(input.ReadBytes((int) dataSize));

                builder.EndMip();
            }

            builder.EndMips();

            return builder.End();
        }
    }
}
