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

        public abstract void Save(DeviceContext context, Stream stream, ImageFileFormat format = ImageFileFormat.Png);
    }
}
