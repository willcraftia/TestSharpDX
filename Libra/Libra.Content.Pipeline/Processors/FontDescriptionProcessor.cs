#region Using

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Libra.PackedVector;
using Libra.Content.Serialization;

#endregion

// ひにけに GD: WpfFont より移植。
// http://blogs.msdn.com/b/ito/archive/2012/02/19/wpf-font-processor.aspx

namespace Libra.Content.Pipeline.Processors
{
    [ContentProcessorAttribute]
    public sealed class FontDescriptionProcessor : ContentProcessor<FontDescription, SpriteFontContent>
    {
        /// <summary>
        /// 半角英数時を生成文字として追加するか
        /// </summary>
        public bool HasAsciiCharacters { get; set; }

        /// <summary>
        /// 全角英数時を生成文字として追加するか
        /// </summary>
        public bool HasZenkakuLatinLetters { get; set; }

        /// <summary>
        /// 特殊記号文字を追加するか
        /// </summary>
        public bool HasSpecialCharacters { get; set; }

        /// <summary>
        /// ひらがな文字を追加するか
        /// </summary>
        public bool HasHiragana { get; set; }

        /// <summary>
        /// カタカナ文字を追加するか
        /// </summary>
        public bool HasKatakana { get; set; }

        /// <summary>
        /// JIS第1水準漢字を追加するか
        /// </summary>
        public bool HasJisKanjiLevel1 { get; set; }

        /// <summary>
        /// JIS第2水準漢字を追加するか
        /// </summary>
        public bool HasJisKanjiLevel2 { get; set; }

        /// <summary>
        /// ギリシャ文字
        /// </summary>
        public bool HasGreekLetters { get; set; }

        /// <summary>
        /// ギリシャ文字
        /// </summary>
        public bool HasCyrillicLetters { get; set; }

        /// <summary>
        /// ギリシャ文字
        /// </summary>
        public bool HasBoxCharacters { get; set; }

        /// <summary>
        /// 追加文字の指定
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 読み込むメッセージファイル名
        /// </summary>
        public string MessageFilenames { get; set; }

        /// <summary>
        /// アウトラインの太さ
        /// </summary>
        public float OutlineThickness { get; set; }

        /// <summary>
        /// アウトラインの色
        /// </summary>
        public Color OutlineColor { get; set; }

        /// <summary>
        /// アウトライン形状
        /// </summary>
        public PenLineJoin OutlineShape { get; set; }

        /// <summary>
        /// アウトライン描画方法
        /// </summary>
        public FontOutlineStroke OutlineStroke { get; set; }

        /// <summary>
        /// 文字色
        /// </summary>
        public Color FontColor { get; set; }

        /// <summary>
        /// グラデーション開始色
        /// </summary>
        public bool UseGradient { get; set; }

        /// <summary>
        /// グラデーション開始色
        /// </summary>
        public Color GradientBeginColor { get; set; }

        /// <summary>
        /// グラデーション終端色
        /// </summary>
        public Color GradientEndColor { get; set; }

        /// <summary>
        /// グラデーション角度
        /// </summary>
        public int GradientAngle { get; set; }

        /// <summary>
        /// 文字テクスチャフォーマット
        /// </summary>
        public FontTextureFormat TextureFormat { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FontDescriptionProcessor()
        {
            // 既定値の設定
            HasAsciiCharacters = true;
            OutlineColor = new Color(64, 64, 64, 255);
            OutlineShape = PenLineJoin.Miter;
            OutlineStroke = FontOutlineStroke.StrokeOverFill;
            FontColor = new Color(255, 255, 255, 255);
            GradientBeginColor = new Color(64, 128, 255, 255);
            GradientEndColor = new Color(0, 0, 128, 255);
            GradientAngle = 90;

            TextureFormat = FontTextureFormat.Color;
        }

        /// <summary>
        /// FontDescriptionを処理する
        /// </summary>
        protected override SpriteFontContent Process(FontDescription input)
        {
            this.input = input;

            // 出力先のFontContentの生成
            fontContnet = new SpriteFontContent();

            // 文字の追加
            AddExtraCharacters();

            // WPFフォントの生成
            CreateWpfFont();

            // 文字グリフの処理
            ProcessGlyphs();

            // その他の情報の設定
            fontContnet.LineSpacing = (int)(glyphTypeface.Height * fontSize);
            fontContnet.Spacing = input.Spacing;
            fontContnet.DefaultCharacter = input.DefaultCharacter;

            return fontContnet;
        }

        /// <summary>
        /// プロセッサーパラメーターで指定された文字をinput.Chractersへ追加する
        /// </summary>
        void AddExtraCharacters()
        {
            // ASCII文字の追加
            if (HasAsciiCharacters)
            {
                for (char c = '\u0020'; c <= '\u007e'; ++c)
                    input.Characters.Add(c);
            }

            // JISコード文字の追加
            if (HasZenkakuLatinLetters) AddCharacters(JisCode.GetLatinLetters());
            if (HasSpecialCharacters)   AddCharacters(JisCode.GetSpecialCharacters());
            if (HasHiragana)            AddCharacters(JisCode.GetHiragana());
            if (HasKatakana)            AddCharacters(JisCode.GetKatakana());
            if (HasJisKanjiLevel1)      AddCharacters(JisCode.GetKanjiLevel1());
            if (HasJisKanjiLevel2)      AddCharacters(JisCode.GetKanjiLevel2());
            if (HasCyrillicLetters)     AddCharacters(JisCode.GetCyrillicLetters());
            if (HasGreekLetters)        AddCharacters(JisCode.GetGreekLetters());
            if (HasBoxCharacters)       AddCharacters(JisCode.GetBoxDrawingCharacters());

            // 指定されたテキストファイル内の文字列を追加
            if (!String.IsNullOrEmpty(Text))
                AddCharacters(Text);

            if (!String.IsNullOrEmpty(MessageFilenames))
            {
                foreach (var token in MessageFilenames.Split(new char[] { ';' }))
                {
                    // 文字列両端の余分な余白と'"'を取り除く
                    var filename = token.Trim();
                    filename = filename.Trim(new char[] { '"' });
                    filename = filename.Trim(); // '"'内の余分な余白も取り除く

                    AddCharacters(filename);
                }
            }

            // DefaultCharacterも忘れずに
            if (input.DefaultCharacter.HasValue)
                input.Characters.Add(input.DefaultCharacter.Value);
        }

        /// <summary>
        /// 指定されたテキストファイル内の文字を追加する
        /// </summary>
        /// <param name="filename">テキストファイル名</param>
        void AddCharacters(string filename)
        {
            // 指定されたファイルから文字列を読み込む
            // FontDescription.Charctarsに追加する
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(String.Format(
                    "MessageFilenamesで指定されたファイル[{0}]が存在しません",
                    Path.GetFullPath(filename)));
            }

            foreach (var line in File.ReadLines(filename, Encoding.Default))
            {
                AddCharacters(line);
            }

            // CPにファイル依存していることを教える
            //context.AddDependency(Path.GetFullPath(filename));
        }

        /// <summary>
        /// 指定された文字コレクションを追加する
        /// </summary>
        void AddCharacters(IEnumerable<char> addingCharacters)
        {
            foreach (var c in addingCharacters)
            {
                input.Characters.Add(c);
            }
        }

        /// <summary>
        /// FontDescriptionからWPFフォントを生成する
        /// </summary>
        void CreateWpfFont()
        {
            // FontDescriptionでのフォントサイズは72DPIで指定されているので
            // WPFのDIU(Device Independent Unit)に変換する
            fontSize = (float)(input.Size * (WpfDiu / 72.0));

            // フォントスタイルの変換
            var fontWeight = ((input.Style & FontDescriptionStyle.Bold) ==
                FontDescriptionStyle.Bold) ? FontWeights.Bold : FontWeights.Regular;
            var fontStyle = ((input.Style & FontDescriptionStyle.Italic) ==
                FontDescriptionStyle.Italic) ? FontStyles.Italic : FontStyles.Normal;

            // Typefaceの生成
            typeface = new Typeface(new FontFamily(input.FontName),
                                    fontStyle, fontWeight, FontStretches.Normal);

            if (typeface == null)
            {
                throw new InvalidOperationException(
                    "フォント\"{0}\"の生成に失敗しました。" +
                    "指定されたフォントがインストールされているか確認してください。");
            }

            // GlyphTypefaceの取得
            if (typeface.TryGetGlyphTypeface(out glyphTypeface) == false)
            {
                throw new InvalidOperationException(
                    "フォント\"{0}\"のGlyphTypeface生成に失敗しました。");
            }
        }

        void ProcessGlyphs()
        {
            // 文字描画に必要な情報設定
            if (UseGradient)
            {
                textBrush = new LinearGradientBrush(
                    ToWpfColor(this.GradientBeginColor),
                    ToWpfColor(this.GradientEndColor),
                    GradientAngle);
            }
            else
            {
                textBrush = new SolidColorBrush(ToWpfColor(FontColor));
            }

            if (OutlineThickness > 0)
            {
                outlinePen = new Pen(new SolidColorBrush(ToWpfColor(OutlineColor)),
                                    OutlineThickness);
                outlinePen.LineJoin = OutlineShape;
            }
            else
            {
                outlinePen = null;
            }

            renderTarget = null;
            drawingVisual = new DrawingVisual();


            // 登録文字をUnicode順に並び替える、これはXNAが実行時に文字グリフを
            // バイナリ検索しているので重要なステップ
            var characters = from c in input.Characters orderby c select c;

            var layouter = new BoxLayouter();

            // 一文字ずつつ描画し、グリフ情報を生成する
            foreach (char c in characters)
            {
                // 文字描画
                var glyphBounds = RenderCharacter(c);

                // ピクセル情報の取得
                int stride = renderTarget.PixelWidth;
                uint[] pixels = new uint[stride * renderTarget.PixelHeight];
                renderTarget.CopyPixels(pixels, stride * sizeof(uint), 0);

                // Black-Boxを取得し、必要な領域の画像イメージを取得
                glyphBounds = NarrowerGlyph(pixels, stride, glyphBounds);
                var blackBox = GetBlackBox(pixels, stride, glyphBounds);
                pixels = new uint[blackBox.Width * blackBox.Height];
                renderTarget.CopyPixels(
                    ToInt32Rect(blackBox), pixels, blackBox.Width * sizeof(uint), 0);

                // カーニング情報の取得
                var kerning = GetKerning(c, blackBox);

                // FontContentへの設定
                fontContnet.CharacterMap.Add(c);
                fontContnet.Kerning.Add(kerning);
                fontContnet.Glyphs.Add(new Rectangle(
                    0, 0, blackBox.Width, blackBox.Height));
                fontContnet.Cropping.Add(new Rectangle(
                    blackBox.X - glyphBounds.X,
                    blackBox.Y - glyphBounds.Y,
                    glyphBounds.Width, glyphBounds.Height));

                // レイアウト用アイテムとして追加
                layouter.Add(new BoxLayoutItem{Bounds = blackBox, Tag = pixels});
            }

            // テクスチャ処理
            ProcessTexture(layouter);
        }

        /// <summary>
        /// 文字グリフからテクスチャを生成する
        /// </summary>
        /// <param name="layouter"></param>
        void ProcessTexture(BoxLayouter layouter)
        {
            // レイアウト、複数の矩形を1つの矩形内に並べる
            int width, height;
            layouter.Layout(out width, out height);

            // 配置後のグリフを画像へ書き込む
            var bitmap = new PixelBitmapContent<Color>(width, height);
            for (int i = 0; i < layouter.Items.Count; ++i)
            {
                // グリフ位置情報の追加
                var rc = fontContnet.Glyphs[i];
                rc.X = layouter.Items[i].Bounds.X;
                rc.Y = layouter.Items[i].Bounds.Y;
                fontContnet.Glyphs[i] = rc;

                // 個々のグリフ画像をひとつの画像へ追加する
                var pixels = layouter.Items[i].Tag as uint[];
                int idx = 0;
                for (int y = 0; y < rc.Height; ++y)
                {
                    for(int x = 0; x < rc.Width; ++x)
                    {
                        int r = (int)((pixels[idx] & 0x00ff0000) >> 16);
                        int g = (int)((pixels[idx] & 0x0000ff00) >>  8);
                        int b = (int)((pixels[idx] & 0x000000ff) >>  0);
                        int a = (int)((pixels[idx] & 0xff000000) >> 24);
                        bitmap.SetPixel(rc.X + x, rc.Y + y, new Color(r, g, b, a));
                        ++idx;
                    }
                }
            }

            // 文字画像をまとめた画像をテクスチャへ変換する
            fontContnet.Texture = new Texture2DContent();
            switch(TextureFormat)
            {
                case FontTextureFormat.Auto:
                    if (UseGradient)
                    {
                        // グラデーション使用していればColorフォーマット
                        fontContnet.Texture.Mipmaps = bitmap;
                    }
                    else if (OutlineThickness > 0)
                    {
                        // アウトラインのみ使用していればBgra4444フォーマット
                        fontContnet.Texture.Mipmaps = ConvertToBgra4444(bitmap);
                    }
                    else
                    {
                        // それ以外の単色フォントであれば単色に特化したDXT3圧縮をする
                        fontContnet.Texture.Mipmaps =
                            SingleColorBlockCompressor.Compress(bitmap, FontColor);
                    }
                    break;
                case FontTextureFormat.Bgra4444:
                    fontContnet.Texture.Mipmaps = ConvertToBgra4444(bitmap);
                    break;
                case FontTextureFormat.Color:
                    fontContnet.Texture.Mipmaps = bitmap;
                    break;
            }

        }

        /// <summary>
        /// カーニング情報の取得
        /// </summary>
        Vector3 GetKerning(char character, Rectangle blackBox)
        {
            // Left/RightBearing情報が取得できれば、その情報を、
            // できなければBlack-box値をカーニング情報として使用する
            Vector3 kerning = Vector3.Zero;
            ushort glyphIdx;
            if (glyphTypeface.CharacterToGlyphMap.TryGetValue(character, out glyphIdx))
            {
                var leftSideBearing = glyphTypeface.LeftSideBearings[glyphIdx];
                var rightSideBearing = glyphTypeface.RightSideBearings[glyphIdx];
                kerning.X = SnapPixel(leftSideBearing * fontSize);
                kerning.Z = SnapPixel(rightSideBearing * fontSize);
            }

            kerning.Y = blackBox.Width;

            return kerning;
        }

        /// <summary>
        /// ピクセル単位のカーニング値取得
        /// </summary>
        static float SnapPixel(double value)
        {
            // WPFの描画結果に合わせる為のバイアス(トライ&エラーの産物)
            var bias = 0.0937456;

            if (value > 0)
                return (float)Math.Floor(value + bias);

            return (float)Math.Ceiling(value - bias);
        }

        /// <summary>
        /// 文字描画に必要なサイズのレンダーターゲットを用意する
        /// </summary>
        /// <param name="width">横幅</param>
        /// <param name="height">高さ</param>
        void EnsureRenderTargetSize(int width, int height)
        {
            if (renderTarget == null ||
                renderTarget.Width < width || renderTarget.Height < height)
            {
                // 32ピクセル単位で生成する
                renderTarget = new RenderTargetBitmap(
                    width + (width % 32), height + (height % 32),
                    WpfDiu, WpfDiu, PixelFormats.Pbgra32);
            }
        }

        /// <summary>
        /// 一文字描画
        /// </summary>
        /// <param name="character"></param>
        /// <remarks>
        /// 独自の文字描画を使用する時はこのメソッドをオーバーライドする
        /// </remarks>
        Rectangle RenderCharacter(char character)
        {
            // フォントサイズの取得
            var formattedText = new FormattedText(
                    new String(character, 1), CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, typeface, fontSize, textBrush);

            // 描画領域の計算。余裕を持って1.5倍のサイズにする
            var width = Math.Max((int)Math.Ceiling(
                                    formattedText.Width * 1.5 + OutlineThickness), 1);
            var height = Math.Max((int)Math.Ceiling(
                                    formattedText.Height * 1.5 + OutlineThickness), 1);

            // レンダーターゲットサイズの確保
            EnsureRenderTargetSize(width, height);

            // 暫定的なグリフ位置の取得
            int fontWidth = Math.Max((int)Math.Ceiling(formattedText.Width), 1);
            int fontHeight = Math.Max((int)Math.Ceiling(formattedText.Height), 1);
            Rectangle rc = new Rectangle(
                (renderTarget.PixelWidth - fontWidth) / 2,
                (renderTarget.PixelHeight - fontHeight) / 2,
                fontWidth, fontHeight);

            // レンダーターゲットへの文字描画
            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                var pos = new System.Windows.Point(rc.X, rc.Y);
                if (outlinePen != null)
                {
                    var geometry = formattedText.BuildGeometry(pos);

                    switch(OutlineStroke)
                    {
                        case FontOutlineStroke.StrokeOverFill:
                            dc.DrawGeometry(textBrush, outlinePen, geometry);
                            break;
                        case FontOutlineStroke.FillOverStroke:
                            dc.DrawGeometry(null, outlinePen, geometry);
                            dc.DrawGeometry(textBrush, null, geometry);
                            break;
                        case FontOutlineStroke.StrokeOnly:
                            dc.DrawGeometry(null, outlinePen, geometry);
                            break;
                    }
                }
                else
                {
                    dc.DrawText(formattedText, pos);
                }
            }
            renderTarget.Clear();
            renderTarget.Render(drawingVisual);

            return rc;
        }

        /// <summary>
        /// 実際の文字グリフ幅を画像から取得する
        /// </summary>
        /// <remarks>
        /// WPF文字描画時では実際の文字グリフ(Black-Box)の周りにBearing値の分だけ
        /// 左右に空き領域ができる場合があるので、ここでは画像データから
        /// Black-Box領域部分を取得している。
        /// また、アウトライン描画等のGeometryを使った文字描画をすると
        /// 文字のBlack-Box領域より大きくなる場合があるので、その場合にも対処している
        /// </remarks>
        Rectangle NarrowerGlyph(uint[] pixels, int stride, Rectangle bounds)
        {
            int left = bounds.X;
            int right = bounds.Right - 1;
            int width = renderTarget.PixelWidth;
            int height = renderTarget.PixelHeight;

            // ピクセルデータがある左端を走査
            while (left > 0 && !IsEmptyColumn(pixels, stride, left, 0, height))
                left--;

            while ((left < right) && IsEmptyColumn(pixels, stride, left, 0, height))
                left++;

            // ピクセルデータがある右端を走査
            while ((right < width) && !IsEmptyColumn(pixels, stride, right, 0, height))
                right++;

            right = Math.Min(right, width - 1);

            while ((left <= right) && IsEmptyColumn(pixels, stride, right, 0, height))
                right--;

            // スペースキャラクターだった(全てが透明色の文字)
            if (right < left)
            {
                left = right = 0;
            }

            // グリフサイズ調整
            bounds.X = left;
            bounds.Width = right - left + 1;

            return bounds;
        }

        /// <summary>
        /// Black-Box領域を取得する
        /// </summary>
        /// <remarks>
        /// NarrowerGlyphを読んだ後に呼ぶこと
        /// NarroerGlyphメソッドでBlack-Boxの左右値は既に求められているので
        /// ここではBlack-Boxの上下端を割り出している
        /// </remarks>
        Rectangle GetBlackBox(uint[] pixels, int stride, Rectangle bounds)
        {
            int x1 = bounds.X;
            int x2 = bounds.Right;
            int top = bounds.Y;
            int bottom = bounds.Bottom - 1;
            int height = renderTarget.PixelHeight;

            // ピクセルデータがある上端を走査
            while ((0 < top) && !IsEmptyLine(pixels, stride, top, x1, x2))
                top--;

            while ((top < bottom) && IsEmptyLine(pixels, stride, top, x1, x2))
                top++;

            // ピクセルデータがある下端を走査
            while ((bottom < height) && !IsEmptyLine(pixels, stride, bottom, x1, x2))
                bottom++;

            bottom = Math.Min(bottom, height- 1);

            while ((top <= bottom) && IsEmptyLine(pixels, stride, bottom, x1, x2))
                bottom--;

            // スペースキャラクターだった(全てが透明色の文字)
            if (bottom < top)
            {
                top = bottom = 0;
            }

            // グリフサイズ調整
            bounds.Y = top;
            bounds.Height = bottom - top + 1;

            return bounds;
        }

        /// <summary>
        /// 画像の指定された列に透明色以外のピクセルが存在するか？
        /// </summary>
        static bool IsEmptyColumn(uint[] pixels, int stride, int x, int y1, int y2)
        {
            var idx = y1 * stride + x;
            for (int y = y1; y < y2; ++y, idx += stride)
            {
                if (pixels[idx] != TransparentPixel)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 画像の指定された行に透明色以外のピクセルが存在するか？
        /// </summary>
        static bool IsEmptyLine(uint[] pixels, int stride, int y, int x1, int x2)
        {
            var idx = y * stride + x1;
            for (int x = x1; x < x2; ++x, ++idx)
            {
                if (pixels[idx] != TransparentPixel)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// XNAのColor構造体からWPFのColorへ変換する
        /// </summary>
        static System.Windows.Media.Color ToWpfColor(Color color)
        {
            return
                System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// XNAのRectangle構造体からInt32Rectへ変換する
        /// </summary>
        static Int32Rect ToInt32Rect(Rectangle rc)
        {
            return new Int32Rect(rc.X, rc.Y, rc.Width, rc.Height);
        }

        static PixelBitmapContent<Bgra4444> ConvertToBgra4444(PixelBitmapContent<Color> source)
        {
            var destination = new PixelBitmapContent<Bgra4444>(source.Width, source.Height);
            
            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    var vector4 = source.GetPixel(x, y).ToVector4();
                    destination.SetPixel(x, y, new Bgra4444(vector4));
                }
            }

            return destination;
        }

        // WPFのDIU(Device Independent Unit)
        const double WpfDiu = 96;

        // 透明色
        const uint TransparentPixel = 0;

        // 処理中のFontDescription
        FontDescription input;

        // 出力先
        SpriteFontContent fontContnet;

        // フォントサイズ(DIU)
        float fontSize;

        // WPFフォント情報
        Typeface typeface;
        GlyphTypeface glyphTypeface;

        // テキスト描画用ブラシ
        Brush textBrush;

        // アウトライン描画用ペン
        Pen outlinePen;

        // 一文字を描画するためのレンダーターゲット
        RenderTargetBitmap renderTarget;

        // 文字描画用のDrawingVisual
        DrawingVisual drawingVisual;
    }
}
