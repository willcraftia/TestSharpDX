#region Using

using System;
using System.Runtime.InteropServices;
using Libra.Graphics;
using Libra.PackedVector;

#endregion

namespace Libra.Content.Compiler
{
    public sealed class PixelBitmapContent<T> : BitmapContent where T : struct
    {
        static readonly int SizeOfT;

        T[] data;

        static PixelBitmapContent()
        {
            SizeOfT = Marshal.SizeOf(typeof(T));
        }

        public PixelBitmapContent(int width, int height)
            : base(width, height)
        {
            data = new T[Width * Height];
        }

        public T GetPixel(int x, int y)
        {
            return data[y * Width + x];
        }

        public void SetPixel(int x, int y, T value)
        {
            data[y * Width + x] = value;
        }

        public override byte[] GetPixelData()
        {
            var size = SizeOfT * data.Length;
            var bytes = new byte[size];
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                Marshal.Copy(handle.AddrOfPinnedObject(), bytes, 0, bytes.Length);
            }
            finally
            {
                handle.Free();
            }

            return bytes;
        }

        public override void SetPixelData(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");

            var size = SizeOfT * data.Length;
            if (size < bytes.Length) throw new ArgumentOutOfRangeException("bytes");

            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                Marshal.Copy(bytes, 0, handle.AddrOfPinnedObject(), bytes.Length);
            }
            finally
            {
                handle.Free();
            }
        }

        public override bool TryGetFormat(out SurfaceFormat format)
        {
            var type = typeof(T);
            
            if (type == typeof(Color))
            {
                format = SurfaceFormat.Color;
                return true;
            }

            if (type == typeof(Bgra4444))
            {
                format = SurfaceFormat.Bgra4444;
                return true;
            }

            // TODO
            //
            // 残りは必要になった時に実装。
            // 基本的には、Vector3 や Vecto4 などの基礎構造体、
            // および、IPackedVector 構造体のみを T 型として許容する前提。

            format = 0;
            return false;
        }
    }
}
