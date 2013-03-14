#region Using

using System;

#endregion

// ひにけに GD: WpfFont より移植。
// http://blogs.msdn.com/b/ito/archive/2012/02/19/wpf-font-processor.aspx

namespace Libra.Content.Compiler
{
    /// <summary>
    /// レイアウト対象になる矩形情報
    /// </summary>
    public sealed class BoxLayoutItem
    {
        /// <summary>
        /// 矩形情報
        /// </summary>
        public Rectangle Bounds { get; set; }

        /// <summary>
        /// タグ
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 配置済みか？
        /// </summary>
        public bool Placed { get; set; }
    }
}
