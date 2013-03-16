#region Using

using System;
using System.IO;

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

        protected Texture2D()
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

        public void GetData<T>(DeviceContext context, int level, T[] data, int startIndex, int elementCount) where T : struct
        {
            context.GetData(this, level, data, startIndex, elementCount);
        }

        public void GetData<T>(DeviceContext context, int level, T[] data) where T : struct
        {
            GetData(context, level, data, 0, data.Length);
        }

        public void GetData<T>(DeviceContext context, T[] data, int startIndex, int elementCount) where T : struct
        {
            GetData(context, 0, data, startIndex, elementCount);
        }

        public void GetData<T>(DeviceContext context, T[] data) where T : struct
        {
            GetData(context, 0, data, 0, data.Length);
        }

        public void SetData<T>(DeviceContext context, T[] data, int startIndex, int elementCount) where T : struct
        {
            SetData(context, 0, data, startIndex, elementCount);
        }

        public void SetData<T>(DeviceContext context, int level, T[] data, int startIndex, int elementCount) where T : struct
        {
            context.SetData(this, level, data, startIndex, elementCount);
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
