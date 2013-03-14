#region Using

using System;

#endregion

// ひにけに GD: WpfFont より移植。
// http://blogs.msdn.com/b/ito/archive/2012/02/19/wpf-font-processor.aspx

namespace Libra.Content.Pipeline.Processors
{
    /// <summary>
    /// フォントプロセッサーで使用するテクスチャフォーマット
    /// </summary>
    public enum FontTextureFormat
    {
        Auto,       // 自動:単色の場合にはDXT3、アウトライン使用でBgra444、
        // グラデーション使用でColorとフォーマットを切り替える
        Color,
        Bgra4444
    }
}
