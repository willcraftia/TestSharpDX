#region Using

using System;
using System.Collections.ObjectModel;

#endregion

namespace Libra.Graphics
{
    // IInputType は一つの入力レイアウトを決定するが、
    // これを実装する型がIInputType が定義するレイアウトに従わずとも良い。
    // なぜならば、本来、入力レイアウトは、
    // より自由にセマンティクスとデータを関連付ける物だからである。
    //
    // IInputType が定めるセマンティクスとは異なる物を用いたい場合は、
    // それらを IInputType によらず定義してレイアウトを構築し、
    // 値をバッファへ設定すれば良い。
    //
    // よって、IInputType により示されるレイアウトは、
    // そこで定義されるセマンティクスを前提とすることで、
    // よりコードを簡素にするためのデフォルトとして振る舞う。

    public interface IInputType
    {
        InputElement[] InputElements { get; }
    }
}
