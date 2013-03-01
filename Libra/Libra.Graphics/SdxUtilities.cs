#region Using

using System;

using SDXUtilities = SharpDX.Utilities;

#endregion

namespace Libra.Graphics
{
    public static class SdxUtilities
    {
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
