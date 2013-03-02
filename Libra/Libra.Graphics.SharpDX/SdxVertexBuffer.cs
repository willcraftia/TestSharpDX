#region Using

using System;

using D3D11Buffer = SharpDX.Direct3D11.Buffer;

#endregion

namespace Libra.Graphics.SharpDX
{
    // TODO
    //
    // XNA の DynamicVertexBuffer に相当する機能は、
    // バッファ サイズの確保の仕方が異なるため、
    // やはり同様に専用のクラスとして定義すべきと思われる。
    //
    // もし DynamicVertexBuffer を作るならば、sealed を外し、
    // Texture2D を参考に拡張可能な状態へ作り直すこと。

    public sealed class SdxVertexBuffer : SdxBuffer, IVertexBuffer
    {
        public SdxVertexBuffer(D3D11Buffer d3d11Buffer)
            : base(d3d11Buffer)
        {
        }
    }
}
