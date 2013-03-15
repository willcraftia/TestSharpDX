﻿#region Using

using System;
using System.Collections.Generic;

#endregion

// ひにけに GD: WpfFont より移植。
// http://blogs.msdn.com/b/ito/archive/2012/02/19/wpf-font-processor.aspx

namespace Libra.Content.Pipeline.Compiler
{
    [ContentTypeWriter]
    public sealed class SpriteFontWriter : ContentTypeWriter<SpriteFontContent>
    {
        ContentTypeWriter textureWriter;

        ContentTypeWriter rectangleListWriter;

        ContentTypeWriter charListWriter;

        ContentTypeWriter vector3Writer;

        protected override void Initialize(ContentTypeWriterManager manager)
        {
            textureWriter = manager.GetTypeWriter(typeof(Texture2DContent));
            rectangleListWriter = manager.GetTypeWriter(typeof(IList<Rectangle>));
            charListWriter = manager.GetTypeWriter(typeof(IList<char>));
            vector3Writer = manager.GetTypeWriter(typeof(IList<Vector3>));

            base.Initialize(manager);
        }

        protected internal override void Write(ContentWriter output, SpriteFontContent value)
        {
            output.WriteObject(value.Texture, textureWriter);
            output.WriteObject(value.Glyphs, rectangleListWriter);
            output.WriteObject(value.Cropping, rectangleListWriter);
            output.WriteObject(value.CharacterMap, charListWriter);
            output.Write(value.LineSpacing);
            output.Write(value.Spacing);
            output.WriteObject(value.Kerning, vector3Writer);
            output.Write(value.DefaultCharacter.HasValue);

            if (value.DefaultCharacter.HasValue)
            {
                output.Write(value.DefaultCharacter.Value);
            }
        }
    }
}
