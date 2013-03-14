#region Using

using System;

#endregion

// ひにけに GD: WpfFont より移植。
// http://blogs.msdn.com/b/ito/archive/2012/02/19/wpf-font-processor.aspx

namespace Libra.Content.Compiler
{
    public sealed class SpriteFontWriter : ContentTypeWriter<SpriteFontContent>
    {
        protected override void Write(ContentWriter output, SpriteFontContent value)
        {
            output.WriteObject(value.Texture);
            output.WriteObject(value.Glyphs);
            output.WriteObject(value.Cropping);
            output.WriteObject(value.CharacterMap);
            output.Write(value.LineSpacing);
            output.Write(value.Spacing);
            output.WriteObject(value.Kerning);
            output.Write(value.DefaultCharacter.HasValue);

            if (value.DefaultCharacter.HasValue)
            {
                output.Write(value.DefaultCharacter.Value);
            }
        }
    }
}
