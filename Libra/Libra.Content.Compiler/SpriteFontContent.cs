#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Libra.Content.Compiler
{
    public sealed class SpriteFontContent
    {
        /// <summary>
        /// テクスチャの取得/設定
        /// </summary>
        public Texture2DContent Texture { get; set; }

        /// <summary>
        /// 文字グリフ領域の取得/設定
        /// </summary>
        public IList<Rectangle> Glyphs { get; set; }

        /// <summary>
        /// 文字グリフクリップ領域の取得/設定
        /// </summary>
        public IList<Rectangle> Cropping { get; set; }

        /// <summary>
        /// 文字マップの取得/設定
        /// </summary>
        public IList<char> CharacterMap { get; set; }

        /// <summary>
        /// 行間スペースの取得/設定
        /// </summary>
        public int LineSpacing { get; set; }

        /// <summary>
        /// 文字間スペースの取得/設定
        /// </summary>
        public float Spacing { get; set; }

        /// <summary>
        /// カーニングの取得/設定
        /// </summary>
        public IList<Vector3> Kerning { get; set; }

        /// <summary>
        /// デフォルト文字の取得/設定
        /// </summary>
        public char? DefaultCharacter { get; set; }

        public SpriteFontContent()
        {
            Texture = new Texture2DContent();
            Glyphs = new List<Rectangle>();
            Cropping = new List<Rectangle>();
            CharacterMap = new List<char>();
            Kerning = new List<Vector3>();
        }
    }
}
