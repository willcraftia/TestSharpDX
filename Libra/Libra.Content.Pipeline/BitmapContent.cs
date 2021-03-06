﻿#region Using

using System;
using Libra.Graphics;

#endregion

namespace Libra.Content.Pipeline
{
    public abstract class BitmapContent
    {
        public int Width { get; private set; }

        public int Height { get; private set; }

        public BitmapContent(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public abstract byte[] GetPixelData();

        public abstract void SetPixelData(byte[] bytes);

        public abstract bool TryGetFormat(out SurfaceFormat format);
    }
}
