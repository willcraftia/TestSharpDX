#region Using

using System;

using SDXUtilities = SharpDX.Utilities;

#endregion

namespace Libra.Graphics.SharpDX
{
    public static class SdxUtilities
    {
        // これは使わない方針。
        // Marshal.SizeOf で統一。
        // sizeof と Marshal.SizeOf は振る舞いの異なる点があるが、
        // その差異は限られているため、注意しながら利用すれば問題ない・・・はず。
        // 例えば、例として出される bool 値の差異などは、
        // そもそもグラフィックス リソースのデータとして用いる事がない
        // (概ね float の 0 or 1 にするであろう)。

        public static int SizeOf<T>() where T : struct
        {
            // SharpDX のドキュメントによると、unsafe での sizeof と等価。
            // なお、sizeof は、ジェネリクス型を指定するとコンパイル不能。

            // sizeof および Marshal.SizeOf について。
            // http://www.codeproject.com/Articles/97711/sizeof-vs-Marshal-SizeOf

            return SDXUtilities.SizeOf<T>();
        }
    }
}
