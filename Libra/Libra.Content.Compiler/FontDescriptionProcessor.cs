#region Using

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using Libra.Content.Serialization;

using DrawingColor = System.Drawing.Color;
using DrawingGraphics = System.Drawing.Graphics;
using DrawingPoint = System.Drawing.Point;
using DrawingRectangle = System.Drawing.Rectangle;

#endregion

// ロジックは DirectX Tool Kit: MakeSpriteFont.TrueTypeImporter を基本に移植。

namespace Libra.Content.Compiler
{
    public sealed class FontDescriptionProcessor : ContentProcessor<FontDescription, SpriteFontContent>
    {
        #region NativeMethods

        static class NativeMethods
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct ABCFloat
            {
                public float A;

                public float B;
                
                public float C;
            }

            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);

            [DllImport("gdi32.dll")]
            public static extern bool GetCharABCWidthsFloat(IntPtr hdc, uint iFirstChar, uint iLastChar, [Out] ABCFloat[] lpABCF);
        }

        #endregion

        #region ArrangedGlyph

        // Internal helper class keeps track of a glyph while it is being arranged.
        class ArrangedGlyph
        {
            public SpriteFontGlyph Source;

            public int X;
            public int Y;

            public int Width;
            public int Height;
        }

        #endregion

        // Size of the temp surface used for GDI+ rasterization.
        const int MaxGlyphSize = 1024;

        public bool PremultiplyAlpha { get; set; }

        protected override SpriteFontContent Process(FontDescription input)
        {
            var output = new SpriteFontContent();

            using (var font = CreateFont(input))
            using (var brush = new SolidBrush(DrawingColor.White))
            using (var stringFormat = new StringFormat(StringFormatFlags.NoFontFallback))
            using (var bitmap = new Bitmap(MaxGlyphSize, MaxGlyphSize, PixelFormat.Format32bppArgb))
            using (var graphics = DrawingGraphics.FromImage(bitmap))
            {
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                foreach (var c in input.Characters)
                {
                    var glyph = ImportGlyph(c, font, brush, stringFormat, bitmap, graphics);
                    
                    output.Glyphs.Add(glyph);
                }

                output.LineSpacing = font.GetHeight();
            }

            foreach (var glyph in output.Glyphs)
            {
                CropGlyph(glyph);
            }

            output.Bitmap = ArrangeGlyphs(output.Glyphs);

            if (PremultiplyAlpha)
            {
                BitmapUtils.PremultiplyAlpha(output.Bitmap);
            }
            output.PremultiplyAlpha = PremultiplyAlpha;

            output.DefaultCharacter = input.DefaultCharacter;

            return output;
        }

        static float PointsToPixels(float points)
        {
            return points * 96 / 72;
        }

        Font CreateFont(FontDescription description)
        {
            var fontName = description.FontName;

            var font = new Font(fontName, PointsToPixels(description.Size), (FontStyle) description.Style, GraphicsUnit.Pixel);
            var fontFamily = font.FontFamily;

            if (fontName.Equals(fontFamily.GetName(CultureInfo.CurrentCulture.LCID), StringComparison.OrdinalIgnoreCase) ||
                fontName.Equals(fontFamily.GetName(CultureInfo.InvariantCulture.LCID), StringComparison.OrdinalIgnoreCase))
            {
                return font;
            }

            foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                if (fontName.Equals(fontFamily.GetName(culture.LCID), StringComparison.OrdinalIgnoreCase))
                {
                    return font;
                }
            }

            throw new InvalidOperationException("Font not found: " + description.FontName);
        }

        static SpriteFontGlyph ImportGlyph(char character, Font font, Brush brush, StringFormat stringFormat, Bitmap bitmap, DrawingGraphics graphics)
        {
            var characterString = character.ToString();

            // Measure the size of this character.
            var size = graphics.MeasureString(characterString, font, DrawingPoint.Empty, stringFormat);

            int characterWidth = (int) Math.Ceiling(size.Width);
            int characterHeight = (int) Math.Ceiling(size.Height);

            // Pad to make sure we capture any overhangs (negative ABC spacing, etc.)
            int padWidth = characterWidth;
            int padHeight = characterHeight / 2;

            int bitmapWidth = characterWidth + padWidth * 2;
            int bitmapHeight = characterHeight + padHeight * 2;

            if (bitmapWidth > MaxGlyphSize || bitmapHeight > MaxGlyphSize)
                throw new Exception("Excessively large glyph won't fit in my lazily implemented fixed size temp surface.");

            // Render the character.
            graphics.Clear(DrawingColor.Black);
            graphics.DrawString(characterString, font, brush, padWidth, padHeight, stringFormat);
            graphics.Flush();

            // Clone the newly rendered image.
            var glyphBitmap = bitmap.Clone(new DrawingRectangle(0, 0, bitmapWidth, bitmapHeight), PixelFormat.Format32bppArgb);

            BitmapUtils.ConvertGreyToAlpha(glyphBitmap);

            // Query its ABC spacing.
            float? abc = GetCharacterWidth(character, font, graphics);

            // Construct the output Glyph object.
            return new SpriteFontGlyph
            {
                Character = character,
                Bitmap = glyphBitmap,
                XOffset = -padWidth,
                XAdvance = abc.HasValue ? padWidth - bitmapWidth + abc.Value : -padWidth,
                YOffset = -padHeight,
            };
        }

        static float? GetCharacterWidth(char character, Font font, DrawingGraphics graphics)
        {
            // Look up the native device context and font handles.
            IntPtr hdc = graphics.GetHdc();

            try
            {
                IntPtr hFont = font.ToHfont();

                try
                {
                    // Select our font into the DC.
                    IntPtr oldFont = NativeMethods.SelectObject(hdc, hFont);

                    try
                    {
                        // Query the character spacing.
                        var result = new NativeMethods.ABCFloat[1];

                        if (NativeMethods.GetCharABCWidthsFloat(hdc, character, character, result))
                        {
                            return result[0].A +
                                   result[0].B +
                                   result[0].C;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    finally
                    {
                        NativeMethods.SelectObject(hdc, oldFont);
                    }
                }
                finally
                {
                    NativeMethods.DeleteObject(hFont);
                }
            }
            finally
            {
                graphics.ReleaseHdc(hdc);
            }
        }

        public static void CropGlyph(SpriteFontGlyph glyph)
        {
            // Crop the top.
            while ((glyph.Cropping.Height > 1) && BitmapUtils.IsAlphaEntirely(0, glyph.Bitmap, new DrawingRectangle(glyph.Cropping.X, glyph.Cropping.Y, glyph.Cropping.Width, 1)))
            {
                glyph.Cropping.Y++;
                glyph.Cropping.Height--;

                glyph.YOffset++;
            }

            // Crop the bottom.
            while ((glyph.Cropping.Height > 1) && BitmapUtils.IsAlphaEntirely(0, glyph.Bitmap, new DrawingRectangle(glyph.Cropping.X, glyph.Cropping.Bottom - 1, glyph.Cropping.Width, 1)))
            {
                glyph.Cropping.Height--;
            }

            // Crop the left.
            while ((glyph.Cropping.Width > 1) && BitmapUtils.IsAlphaEntirely(0, glyph.Bitmap, new DrawingRectangle(glyph.Cropping.X, glyph.Cropping.Y, 1, glyph.Cropping.Height)))
            {
                glyph.Cropping.X++;
                glyph.Cropping.Width--;

                glyph.XOffset++;
            }

            // Crop the right.
            while ((glyph.Cropping.Width > 1) && BitmapUtils.IsAlphaEntirely(0, glyph.Bitmap, new DrawingRectangle(glyph.Cropping.Right - 1, glyph.Cropping.Y, 1, glyph.Cropping.Height)))
            {
                glyph.Cropping.Width--;

                glyph.XAdvance++;
            }
        }

        public static Bitmap ArrangeGlyphs(IList<SpriteFontGlyph> sourceGlyphs)
        {
            // Build up a list of all the glyphs needing to be arranged.
            var glyphs = new List<ArrangedGlyph>();

            foreach (var sourceGlyph in sourceGlyphs)
            {
                var glyph = new ArrangedGlyph();

                glyph.Source = sourceGlyph;

                // Leave a one pixel border around every glyph in the output bitmap.
                glyph.Width = sourceGlyph.Cropping.Width + 2;
                glyph.Height = sourceGlyph.Cropping.Height + 2;

                glyphs.Add(glyph);
            }

            // Sort so the largest glyphs get arranged first.
            glyphs.Sort(CompareGlyphSizes);

            // Work out how big the output bitmap should be.
            int outputWidth = GuessOutputWidth(sourceGlyphs);
            int outputHeight = 0;

            // Choose positions for each glyph, one at a time.
            for (int i = 0; i < glyphs.Count; i++)
            {
                PositionGlyph(glyphs, i, outputWidth);

                outputHeight = Math.Max(outputHeight, glyphs[i].Y + glyphs[i].Height);
            }

            // Create the merged output bitmap.
            outputHeight = MakeValidTextureSize(outputHeight, false);

            return CopyGlyphsToOutput(glyphs, outputWidth, outputHeight);
        }

        // Comparison function for sorting glyphs by size.
        static int CompareGlyphSizes(ArrangedGlyph a, ArrangedGlyph b)
        {
            const int heightWeight = 1024;

            int aSize = a.Height * heightWeight + a.Width;
            int bSize = b.Height * heightWeight + b.Width;

            if (aSize != bSize)
                return bSize.CompareTo(aSize);
            else
                return a.Source.Character.CompareTo(b.Source.Character);
        }

        // Heuristic guesses what might be a good output width for a list of glyphs.
        static int GuessOutputWidth(IList<SpriteFontGlyph> sourceGlyphs)
        {
            int maxWidth = 0;
            int totalSize = 0;

            foreach (var glyph in sourceGlyphs)
            {
                maxWidth = Math.Max(maxWidth, glyph.Cropping.Width);
                totalSize += glyph.Cropping.Width * glyph.Cropping.Height;
            }

            int width = Math.Max((int) Math.Sqrt(totalSize), maxWidth);

            return MakeValidTextureSize(width, true);
        }

        // Rounds a value up to the next larger valid texture size.
        static int MakeValidTextureSize(int value, bool requirePowerOfTwo)
        {
            // In case we want to DXT compress, make sure the size is a multiple of 4.
            const int blockSize = 4;

            if (requirePowerOfTwo)
            {
                // Round up to a power of two.
                int powerOfTwo = blockSize;

                while (powerOfTwo < value)
                    powerOfTwo <<= 1;

                return powerOfTwo;
            }
            else
            {
                // Round up to the specified block size.
                return (value + blockSize - 1) & ~(blockSize - 1);
            }
        }

        // Works out where to position a single glyph.
        static void PositionGlyph(List<ArrangedGlyph> glyphs, int index, int outputWidth)
        {
            int x = 0;
            int y = 0;

            while (true)
            {
                // Is this position free for us to use?
                int intersects = FindIntersectingGlyph(glyphs, index, x, y);

                if (intersects < 0)
                {
                    glyphs[index].X = x;
                    glyphs[index].Y = y;

                    return;
                }

                // Skip past the existing glyph that we collided with.
                x = glyphs[intersects].X + glyphs[intersects].Width;

                // If we ran out of room to move to the right, try the next line down instead.
                if (x + glyphs[index].Width > outputWidth)
                {
                    x = 0;
                    y++;
                }
            }
        }

        // Checks if a proposed glyph position collides with anything that we already arranged.
        static int FindIntersectingGlyph(List<ArrangedGlyph> glyphs, int index, int x, int y)
        {
            int w = glyphs[index].Width;
            int h = glyphs[index].Height;

            for (int i = 0; i < index; i++)
            {
                if (glyphs[i].X >= x + w)
                    continue;

                if (glyphs[i].X + glyphs[i].Width <= x)
                    continue;

                if (glyphs[i].Y >= y + h)
                    continue;

                if (glyphs[i].Y + glyphs[i].Height <= y)
                    continue;

                return i;
            }

            return -1;
        }

        // Once arranging is complete, copies each glyph to its chosen position in the single larger output bitmap.
        static Bitmap CopyGlyphsToOutput(List<ArrangedGlyph> glyphs, int width, int height)
        {
            var output = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            foreach (var glyph in glyphs)
            {
                var sourceGlyph = glyph.Source;
                var sourceCropping = sourceGlyph.Cropping.ToDrawingRectangle();
                var destinationCropping = new DrawingRectangle(glyph.X + 1, glyph.Y + 1, sourceCropping.Width, sourceCropping.Height);

                BitmapUtils.CopyRect(sourceGlyph.Bitmap, sourceCropping, output, destinationCropping);

                BitmapUtils.PadBorderPixels(output, destinationCropping);

                sourceGlyph.Bitmap = output;
                sourceGlyph.Cropping.FromDrawingRectangle(destinationCropping);
            }

            return output;
        }
    }
}
