#region Using

using System;
using System.IO;
using System.Runtime.InteropServices;

#endregion

namespace Libra.Graphics
{
    public abstract class Texture2D : Resource
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public int MipLevels { get; set; }

        public SurfaceFormat Format { get; set; }

        public int MultisampleCount { get; set; }

        public int MultisampleQuality { get; protected set; }

        protected Texture2D(IDevice device)
            : base(device)
        {
            MipLevels = 1;
            Format = SurfaceFormat.Color;
            MultisampleCount = 1;
            MultisampleQuality = 0;
        }

        public abstract void Initialize();

        public abstract void Initialize(Stream stream);

        public void Initialize(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                Initialize(stream);
            }
        }

        public abstract void Save(DeviceContext context, Stream stream, ImageFileFormat format = ImageFileFormat.Png);

        // GetData メソッドは、デバッグ目的と位置付ける。
        // データ取得のために内部で Staging リソースをインスタンス化し、
        // データ取得後に破棄するため、GetData の頻繁な呼び出しは GC 負荷となり得る。

        public abstract void GetData<T>(
            DeviceContext context, int level, Rectangle? rectangle, T[] data, int startIndex, int elementCount) where T : struct;

        public void GetData<T>(DeviceContext context, int level, T[] data) where T : struct
        {
            GetData(context, level, null, data, 0, data.Length);
        }

        public void GetData<T>(DeviceContext context, T[] data, int startIndex, int elementCount) where T : struct
        {
            GetData(context, 0, null, data, startIndex, elementCount);
        }

        public void GetData<T>(DeviceContext context, T[] data) where T : struct
        {
            GetData(context, 0, null, data, 0, data.Length);
        }

        public void SetData<T>(DeviceContext context, T[] data, int startIndex, int elementCount) where T : struct
        {
            SetData(context, 0, data, startIndex, elementCount);
        }

        public void SetData<T>(DeviceContext context, int level, T[] data, int startIndex, int elementCount) where T : struct
        {
            if (data == null) throw new ArgumentNullException("data");

            if (Usage == ResourceUsage.Immutable)
                throw new InvalidOperationException("Data can not be set from CPU.");

            var gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                var dataPointer = gcHandle.AddrOfPinnedObject();
                var sizeOfT = Marshal.SizeOf(typeof(T));

                var sourcePointer = (IntPtr) (dataPointer + startIndex * sizeOfT);

                if (Usage == ResourceUsage.Default)
                {
                    // TODO
                    //
                    // Immutable と Dynamic 以外は UpdateSubresource で更新可能。
                    // Staging は Map/Unmap で行えるので、Default の場合にのみ UpdateSubresource で更新。
                    // それで良いのか？

                    int rowPitch = FormatHelper.SizeInBytes(Format) * Width;
                    context.UpdateSubresource(this, level, null, sourcePointer, rowPitch, 0);
                }
                else
                {
                    var sizeInBytes = ((elementCount == 0) ? data.Length : elementCount) * sizeOfT;
                    
                    // ポインタの移動に用いるため、フォーマットから測れる要素サイズで算出しなければならない。
                    // SizeOf(typeof(T)) では、例えばバイト配列などを渡した場合に、
                    // そのサイズは元配列の要素の移動となり、リソース要素の移動にはならない。
                    var rowSpan = FormatHelper.SizeInBytes(Format) * Width;

                    // TODO
                    //
                    // Dynamic だと D3D11MapMode.Write はエラーになる。
                    // 対応関係を MSDN から把握できないが、どうすべきか。
                    // ひとまず WriteDiscard とする。

                    var mappedResource = context.Map(this, level, DeviceContext.MapMode.WriteDiscard);
                    try
                    {
                        var rowSourcePointer = sourcePointer;
                        var destinationPointer = mappedResource.Pointer;

                        for (int i = 0; i < Height; i++)
                        {
                            GraphicsHelper.CopyMemory(destinationPointer, rowSourcePointer, rowSpan);
                            destinationPointer += mappedResource.RowPitch;
                            rowSourcePointer += rowSpan;
                        }
                    }
                    finally
                    {
                        context.Unmap(this, level);
                    }
                }
            }
            finally
            {
                gcHandle.Free();
            }

        }

        public void SetData<T>(DeviceContext context, params T[] data) where T : struct
        {
            SetData(context, 0, data, 0, data.Length);
        }

        public void SetData<T>(DeviceContext context, int level, params T[] data) where T : struct
        {
            SetData(context, level, data, 0, data.Length);
        }
    }
}
