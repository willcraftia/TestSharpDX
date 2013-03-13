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
            public SpriteFontGlyph Target;

            public PixelBitmapContent<Color> Bitmap;

            public int X;
            public int Y;

            public int Width;
            public int Height;
        }

        #endregion

        // Size of the temp surface used for GDI+ rasterization.
        const int MaxGlyphSize = 1024;

        SpriteFontContent spriteFontContent;

        List<PixelBitmapContent<Color>> glyphBitmaps;

        List<ArrangedGlyph> arrangedGlyphs;

        PixelBitmapContent<Color> spriteBitmap;

        public bool PremultiplyAlpha { get; set; }

        public FontDescriptionProcessor()
        {
            glyphBitmaps = new List<PixelBitmapContent<Color>>();
            arrangedGlyphs = new List<ArrangedGlyph>();
        }

        protected override SpriteFontContent Process(FontDescription input)
        {
            spriteFontContent = new SpriteFontContent();

            ImportGlyphs(input);
            CropGlyphs();
            ArrangeGlyphs();

            if (PremultiplyAlpha)
            {
                DoPremultiplyAlpha();
            }

            // TODO
            //
            // 他のフォーマットへの変換。

            spriteFontContent.Bitmap = spriteBitmap;
            spriteFontContent.PremultiplyAlpha = PremultiplyAlpha;

            spriteFontContent.DefaultCharacter = input.DefaultCharacter;

            return spriteFontContent;
        }

        void ImportGlyphs(FontDescription fontDescription)
        {
            using (var font = CreateFont(fontDescription))
            using (var brush = new SolidBrush(DrawingColor.White))
            using (var stringFormat = new StringFormat(StringFormatFlags.NoFontFallback))
            using (var bitmap = new Bitmap(MaxGlyphSize, MaxGlyphSize, PixelFormat.Format32bppArgb))
            using (var graphics = DrawingGraphics.FromImage(bitmap))
            {
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                foreach (var c in fontDescription.Characters)
                {
                    ImportGlyph(c, font, brush, stringFormat, bitmap, graphics);
                }

                spriteFontContent.LineSpacing = font.GetHeight();
            }
        }

        void CropGlyphs()
        {
            for (int i = 0; i < spriteFontContent.Glyphs.Count; i++)
            {
                var glyph = spriteFontContent.Glyphs[i];
                var glyphBitmap = glyphBitmaps[i];
                CropGlyph(glyph, glyphBitmap);
            }
        }

        float PointsToPixels(float points)
        {
            return points * 96 / 72;
        }

        Font CreateFont(FontDescription fontDescription)
        {
            var fontName = fontDescription.FontName;

            var font = new Font(
                fontName,
                PointsToPixels(fontDescription.Size),
                (FontStyle) fontDescription.Style,
                GraphicsUnit.Pixel);
            
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

            throw new InvalidOperationException("Font not found: " + fontDescription.FontName);
        }

        void ImportGlyph(char character, Font font, Brush brush, StringFormat stringFormat, Bitmap bitmap, DrawingGraphics graphics)
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

            // Bitmap の内容を PixelBitmapContent へコピー。
            var glyphBitmap = new PixelBitmapContent<Color>(bitmapWidth, bitmapHeight);
            for (int y = 0; y < bitmapHeight; y++)
            {
                for (int x = 0; x < bitmapWidth; x++)
                {
                    var drawingColor = bitmap.GetPixel(x, y);
                    var color = new Color(drawingColor.R, drawingColor.G, drawingColor.B, drawingColor.A);
                    glyphBitmap.SetPixel(x, y, color);
                }
            }

            ConvertGreyToAlpha(glyphBitmap);

            // Query its ABC spacing.
            float? abc = GetCharacterWidth(character, font, graphics);

            var glyph = new SpriteFontGlyph
            {
                Character = character,
                XOffset = -padWidth,
                XAdvance = abc.HasValue ? padWidth - bitmapWidth + abc.Value : -padWidth,
                YOffset = -padHeight,
            };
            spriteFontContent.Glyphs.Add(glyph);

            glyphBitmaps.Add(glyphBitmap);
        }

        void ConvertGreyToAlpha(PixelBitmapContent<Color> bitmap)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var color = bitmap.GetPixel(x, y);

                    // Average the red, green and blue values to compute brightness.
                    int alpha = (color.R + color.G + color.B) / 3;

                    bitmap.SetPixel(x, y, new Color(255, 255, 255, alpha));
                }
            }
        }

        float? GetCharacterWidth(char character, Font font, DrawingGraphics graphics)
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

        void CropGlyph(SpriteFontGlyph glyph, PixelBitmapContent<Color> glyphBitmap)
        {
            // Crop the top.
            while ((glyph.Cropping.Height > 1) &&
                IsAlphaEntirely(0, glyphBitmap, new Rectangle(glyph.Cropping.X, glyph.Cropping.Y, glyph.Cropping.Width, 1)))
            {
                glyph.Cropping.Y++;
                glyph.Cropping.Height--;

                glyph.YOffset++;
            }

            // Crop the bottom.
            while ((glyph.Cropping.Height > 1) &&
                IsAlphaEntirely(0, glyphBitmap, new Rectangle(glyph.Cropping.X, glyph.Cropping.Bottom - 1, glyph.Cropping.Width, 1)))
            {
                glyph.Cropping.Height--;
            }

            // Crop the left.
            while ((glyph.Cropping.Width > 1) &&
                IsAlphaEntirely(0, glyphBitmap, new Rectangle(glyph.Cropping.X, glyph.Cropping.Y, 1, glyph.Cropping.Height)))
            {
                glyph.Cropping.X++;
                glyph.Cropping.Width--;

                glyph.XOffset++;
            }

            // Crop the right.
            while ((glyph.Cropping.Width > 1) &&
                IsAlphaEntirely(0, glyphBitmap, new Rectangle(glyph.Cropping.Right - 1, glyph.Cropping.Y, 1, glyph.Cropping.Height)))
            {
                glyph.Cropping.Width--;

                glyph.XAdvance++;
            }
        }

        bool IsAlphaEntirely(byte expectedAlpha, PixelBitmapContent<Color> bitmap, Rectangle region)
        {
            for (int y = region.Y; y < region.Y + region.Height; y++)
            {
                for (int x = region.X; x < region.X + region.Width; x++)
                {
                    var a = bitmap.GetPixel(x, y).A;
                    if (a != expectedAlpha)
                        return false;
                }
            }

            return true;
        }

        void ArrangeGlyphs()
        {
            // Build up a list of all the glyphs needing to be arranged.
            for (int i = 0; i < spriteFontContent.Glyphs.Count; i++)
            {
                var glyph = spriteFontContent.Glyphs[i];
                var glyphBitmap = glyphBitmaps[i];

                // Leave a one pixel border around every glyph in the output bitmap.
                var arrangedGlyph = new ArrangedGlyph
                {
                    Target = glyph,
                    Bitmap = glyphBitmap,
                    Width = glyph.Cropping.Width + 2,
                    Height = glyph.Cropping.Height + 2
                };
                arrangedGlyphs.Add(arrangedGlyph);
            }

            // Sort so the largest glyphs get arranged first.
            arrangedGlyphs.Sort(CompareGlyphSizes);

            // Work out how big the output bitmap should be.
            int outputWidth = GuessOutputWidth(spriteFontContent.Glyphs);
            int outputHeight = 0;

            // Choose positions for each glyph, one at a time.
            for (int i = 0; i < arrangedGlyphs.Count; i++)
            {
                PositionGlyph(i, outputWidth);

                outputHeight = Math.Max(outputHeight, arrangedGlyphs[i].Y + arrangedGlyphs[i].Height);
            }

            // Create the merged output bitmap.
            outputHeight = MakeValidTextureSize(outputHeight, false);

            spriteBitmap = new PixelBitmapContent<Color>(outputWidth, outputHeight);

            for (int i = 0; i < arrangedGlyphs.Count; i++)
            {
                var arrangedGlyph = arrangedGlyphs[i];
                var glyph = arrangedGlyph.Target;
                var glyphBitmap = arrangedGlyph.Bitmap;
                var sourceRegion = glyph.Cropping;
                var destinationRegion = new Rectangle(arrangedGlyph.X + 1, arrangedGlyph.Y + 1, sourceRegion.Width, sourceRegion.Height);

                CopyRect(glyphBitmap, sourceRegion, spriteBitmap, destinationRegion);

                PadBorderPixels(spriteBitmap, destinationRegion);

                glyph.Cropping = destinationRegion;
            }
        }

        // Comparison function for sorting glyphs by size.
        int CompareGlyphSizes(ArrangedGlyph a, ArrangedGlyph b)
        {
            const int heightWeight = 1024;

            int aSize = a.Height * heightWeight + a.Width;
            int bSize = b.Height * heightWeight + b.Width;

            if (aSize != bSize)
                return bSize.CompareTo(aSize);
            else
                return a.Target.Character.CompareTo(b.Target.Character);
        }

        // Heuristic guesses what might be a good output width for a list of glyphs.
        int GuessOutputWidth(IList<SpriteFontGlyph> sourceGlyphs)
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
        int MakeValidTextureSize(int value, bool requirePowerOfTwo)
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
        void PositionGlyph(int index, int outputWidth)
        {
            int x = 0;
            int y = 0;

            while (true)
            {
                // Is this position free for us to use?
                int intersects = FindIntersectingGlyph(index, x, y);

                if (intersects < 0)
                {
                    arrangedGlyphs[index].X = x;
                    arrangedGlyphs[index].Y = y;

                    return;
                }

                // Skip past the existing glyph that we collided with.
                x = arrangedGlyphs[intersects].X + arrangedGlyphs[intersects].Width;

                // If we ran out of room to move to the right, try the next line down instead.
                if (x + arrangedGlyphs[index].Width > outputWidth)
                {
                    x = 0;
                    y++;
                }
            }
        }

        // Checks if a proposed glyph position collides with anything that we already arranged.
        int FindIntersectingGlyph(int index, int x, int y)
        {
            int w = arrangedGlyphs[index].Width;
            int h = arrangedGlyphs[index].Height;

            for (int i = 0; i < index; i++)
            {
                if (arrangedGlyphs[i].X >= x + w)
                    continue;

                if (arrangedGlyphs[i].X + arrangedGlyphs[i].Width <= x)
                    continue;

                if (arrangedGlyphs[i].Y >= y + h)
                    continue;

                if (arrangedGlyphs[i].Y + arrangedGlyphs[i].Height <= y)
                    continue;

                return i;
            }

            return -1;
        }

        void CopyRect(
            PixelBitmapContent<Color> source, Rectangle sourceRegion,
            PixelBitmapContent<Color> destination, Rectangle destinationRegion)
        {
            if (sourceRegion.Width != destinationRegion.Width ||
                sourceRegion.Height != destinationRegion.Height)
            {
                throw new ArgumentException();
            }

            for (int y = 0; y < sourceRegion.Height; y++)
            {
                for (int x = 0; x < sourceRegion.Width; x++)
                {
                    var color = source.GetPixel(sourceRegion.X + x, sourceRegion.Y + y);
                    destination.SetPixel(destinationRegion.X + x, destinationRegion.Y + y, color);
                }
            }
        }

        void PadBorderPixels(PixelBitmapContent<Color> bitmap, Rectangle region)
        {
            // Pad the top and bottom.
            for (int x = region.Left; x < region.Right; x++)
            {
                CopyBorderPixel(bitmap, x, region.Top,        x, region.Top - 1);
                CopyBorderPixel(bitmap, x, region.Bottom - 1, x, region.Bottom);
            }

            // Pad the left and right.
            for (int y = region.Top; y < region.Bottom; y++)
            {
                CopyBorderPixel(bitmap, region.Left,      y, region.Left - 1, y);
                CopyBorderPixel(bitmap, region.Right - 1, y, region.Right,    y);
            }

            // Pad the four corners.
            CopyBorderPixel(bitmap, region.Left,      region.Top,        region.Left - 1, region.Top - 1);
            CopyBorderPixel(bitmap, region.Right - 1, region.Top,        region.Right,    region.Top - 1);
            CopyBorderPixel(bitmap, region.Left,      region.Bottom - 1, region.Left - 1, region.Bottom);
            CopyBorderPixel(bitmap, region.Right - 1, region.Bottom - 1, region.Right,    region.Bottom);
        }

        void CopyBorderPixel(PixelBitmapContent<Color> bitmap, int sourceX, int sourceY, int destX, int destY)
        {
            var color = bitmap.GetPixel(sourceX, sourceY);
            bitmap.SetPixel(destX, destY, new Color(color.R, color.G, color.B, 0));
        }

        void DoPremultiplyAlpha()
        {
            for (int y = 0; y < spriteBitmap.Height; y++)
            {
                for (int x = 0; x < spriteBitmap.Width; x++)
                {
                    var color = spriteBitmap.GetPixel(x, y);

                    int a = color.A;
                    int r = color.R * a / 255;
                    int g = color.G * a / 255;
                    int b = color.B * a / 255;

                    spriteBitmap.SetPixel(x, y, new Color(r, g, b, a));
                }
            }
        }

        // TODO
        //
        //

    }
}
