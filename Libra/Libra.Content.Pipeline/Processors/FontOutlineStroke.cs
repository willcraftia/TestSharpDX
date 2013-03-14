#region Using

using System;

#endregion

// ひにけに GD: WpfFont より移植。
// http://blogs.msdn.com/b/ito/archive/2012/02/19/wpf-font-processor.aspx

namespace Libra.Content.Pipeline.Processors
{
    /// <summary>
    /// アウトライン描画方法
    /// </summary>
    public enum FontOutlineStroke
    {
        StrokeOverFill, // 文字本体描画の後にアウトラインを描画する
        FillOverStroke, // アウトラインを描画した後に文字本体描画する
        StrokeOnly      // アウトラインのみを描画する
    }
}
