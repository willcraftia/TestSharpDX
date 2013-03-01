#region Using

using System;

using D3D11BindFlags = SharpDX.Direct3D11.BindFlags;
using D3D11Buffer = SharpDX.Direct3D11.Buffer;
using D3D11BufferDescription = SharpDX.Direct3D11.BufferDescription;

#endregion

namespace Libra.Graphics
{
    // TODO
    //
    // XNA の DynamicVertexBuffer に相当する機能は、
    // バッファ サイズの確保の仕方が異なるため、
    // やはり同様に専用のクラスとして定義すべきと思われる。
    //
    // もし DynamicVertexBuffer を作るならば、sealed を外し、
    // Texture2D を参考に拡張可能な状態へ作り直すこと。

    public sealed class VertexBuffer : Buffer
    {
        public VertexBuffer(Device device, int sizeInBytes, ResourceUsage usage)
            : base(device, CreateD3D11Buffer(device, sizeInBytes, usage))
        {
        }

        internal VertexBuffer(Device device, D3D11Buffer d3d11Buffer)
            : base(device, d3d11Buffer)
        {
        }

        // デフォルト値は ResourceUsage.Immutable。
        // 殆どの場合、非 Immutable ならば公開コンストラクタから生成し、
        // インスタンス化の後にデータを設定する必要があると思われる。

        public static VertexBuffer Create<T>(Device device, T[] data, ResourceUsage usage = ResourceUsage.Immutable)
            where T : struct
        {
            // 「要素の構造体のサイズ」×「配列サイズ」がバッファのサイズ。
            int sizeInBytes = SdxUtilities.SizeOf<T>() * data.Length;
            D3D11BufferDescription description;
            CreateD3D11BufferDescription(D3D11BindFlags.VertexBuffer, sizeInBytes, usage, out description);

            var d3d11Buffer = D3D11Buffer.Create(device.D3D11Device, data, description);
            return new VertexBuffer(device, d3d11Buffer);
        }

        static D3D11Buffer CreateD3D11Buffer(Device device, int sizeInBytes, ResourceUsage usage)
        {
            // 初期データ無しの構築であるため Immutable は禁止。
            if (usage == ResourceUsage.Immutable)
                throw new ArgumentException("Usage must not be immutable.", "usage");

            D3D11BufferDescription description;
            CreateD3D11BufferDescription(D3D11BindFlags.VertexBuffer, sizeInBytes, usage, out description);

            return new D3D11Buffer(device.D3D11Device, description);
        }
    }
}
