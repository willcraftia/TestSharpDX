#region Using

using System;

#endregion

namespace Libra.Content.Pipeline
{
    public abstract class BCBitmapContent : BitmapContent
    {
        byte[] data;

        protected BCBitmapContent(int width, int height, int blockSize)
            : base(width, height)
        {
            data = new byte[width * height];
        }

        public override byte[] GetPixelData()
        {
            var result = new byte[data.Length];
            Array.Copy(data, result, data.Length);
            return result;
        }

        public override void SetPixelData(byte[] bytes)
        {
            if (bytes.Length != data.Length) throw new ArgumentOutOfRangeException("bytes");

            Array.Copy(bytes, data, bytes.Length);
        }
    }
}
