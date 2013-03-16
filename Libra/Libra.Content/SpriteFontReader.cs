#region Using

using System;
using System.Collections.Generic;
using Libra.Graphics;

#endregion

namespace Libra.Content
{
    [ContentTypeReader]
    public sealed class SpriteFontReader : ContentTypeReader<SpriteFont>
    {
        protected internal override SpriteFont Read(ContentReader input, SpriteFont existingInstance)
        {
            var texture2D = input.ReadObject<Texture2D>();
            var bounds = input.ReadObject<IList<Rectangle>>();
            var cropping = input.ReadObject<IList<Rectangle>>();
            var characters = input.ReadObject<IList<char>>();
            var lineSpacing = input.ReadInt32();
            var spacing = input.ReadSingle();
            var kerning = input.ReadObject<IList<Vector3>>();
            char? defaultCharacter = null;
            if (input.ReadBoolean())
            {
                defaultCharacter = input.ReadChar();
            }

            var device = input.Device;
            var texture = device.CreateShaderResourceView();
            texture.Initialize(texture2D);

            return new SpriteFont(texture, bounds, cropping, characters, lineSpacing, spacing, kerning, defaultCharacter);
        }
    }
}
