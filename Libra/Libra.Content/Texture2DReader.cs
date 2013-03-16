#region Using

using System;
using Libra.Graphics;

#endregion

namespace Libra.Content
{
    [ContentTypeReader]
    public sealed class Texture2DReader : ContentTypeReader<Texture2D>
    {
        // Int32:   Surface format (DXGI format value)
        // UInt32:  Width
        // UInt32:  Height
        // UInt32:  Mip count
        // Repeat<mip count>
        // {
        //      UInt32:             Data size
        //      Byte[data size]:    Image data
        // }

        protected internal override Texture2D Read(ContentReader input, Texture2D existingInstance)
        {
            var texture = input.Device.CreateTexture2D();

            texture.Format = (SurfaceFormat) input.ReadInt32();
            texture.Width = (int) input.ReadUInt32();
            texture.Height = (int) input.ReadUInt32();
            texture.MipLevels = (int) input.ReadUInt32();

            // ResourceUsage の決定が難しい。
            //
            // 候補としては Default ないしは Immutable。
            // Dynamic は、アプリケーションでデータを更新するという特性から、
            // コンテンツのロードを基点とせずにインスタンスを生成する場合が殆どと仮定してよい。
            // Staging は、まだ使い道を理解していないが、
            // ロードしたコンテンツの内容を読み込むためならば Default で済む。
            //
            // 仮に、ResourceUsage を厳密に決定するならば、
            // バイナリ データに ResourceUsage フィールドを含めるか、
            // 追加パラメータの指定を許容するようなインタフェースとする必要がある。
            //
            // 当面、Default 固定で生成する。
            texture.Usage = ResourceUsage.Default;

            // TODO
            //
            // マルチサンプリングの設定はどうするのだ？
            // 当面、デフォルト (マルチサンプリングなし) で生成しておく・・・。

            texture.Initialize();

            var context = input.DeviceContext;
            for (int i = 0; i < texture.MipLevels; i++)
            {
                var size = (int) input.ReadUInt32();
                var bytes = input.ReadBytes(size);

                texture.SetData(context, 0, bytes);
            }

            return texture;
        }
    }
}
