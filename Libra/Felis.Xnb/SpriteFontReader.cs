#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public sealed class SpriteFontReader : TypeReader
    {
        SpriteFontBuilderBase builder;

        public override string TargetType
        {
            get { return "Microsoft.Xna.Framework.Graphics.SpriteFont"; }
        }

        public override string ReaderName
        {
            get { return "Microsoft.Xna.Framework.Content.SpriteFontReader"; }
        }

        protected internal override void Initialize(TypeReaderManager manager)
        {
            builder = manager.GetTypeBuilderByTargetType(TargetType) as SpriteFontBuilderBase;

            base.Initialize(manager);
        }

        protected internal override object Read(XnbReader input)
        {
            builder.Begin();

            // Texture
            builder.SetTexture(input.ReadObject());

            // Glyphs
            builder.SetGlyphs(input.ReadObject());

            // Cropping
            builder.SetCropping(input.ReadObject());

            // Character map
            builder.SetCharacterMap(input.ReadObject());

            // Vertical line spacing
            builder.SetVerticalLineSpacing(input.ReadInt32());

            // Horizontal spacing
            builder.SetHorizontalSpacing(input.ReadSingle());

            // Kerning
            builder.SetKering(input.ReadObject());

            // Default character
            if (input.ReadBoolean())
            {
                builder.SetDefaultCharacter(input.ReadChar());
            }
            else
            {
                builder.SetDefaultCharacter(null);
            }

            return builder.End();
        }
    }
}
