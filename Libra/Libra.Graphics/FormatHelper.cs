﻿#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public static class FormatHelper
    {
        static readonly int[] sizeInBits = new int[256];

        static FormatHelper()
        {
            sizeInBits[(int) SurfaceFormat.Color] = 32;
            sizeInBits[(int) SurfaceFormat.Bgr565] = 16;
            sizeInBits[(int) SurfaceFormat.Bgra5551] = 16;
            sizeInBits[(int) SurfaceFormat.Bgra4444] = 16;
            sizeInBits[(int) SurfaceFormat.BC1] = 4;
            sizeInBits[(int) SurfaceFormat.BC2] = 8;
            sizeInBits[(int) SurfaceFormat.BC3] = 8;
            sizeInBits[(int) SurfaceFormat.NormalizedByte2] = 16;
            sizeInBits[(int) SurfaceFormat.NormalizedByte4] = 32;
            sizeInBits[(int) SurfaceFormat.Rgba1010102] = 32;
            sizeInBits[(int) SurfaceFormat.Rg32] = 32;
            sizeInBits[(int) SurfaceFormat.Rgba64] = 64;
            sizeInBits[(int) SurfaceFormat.Alpha8] = 8;
            sizeInBits[(int) SurfaceFormat.Single] = 32;
            sizeInBits[(int) SurfaceFormat.Vector2] = 64;
            sizeInBits[(int) SurfaceFormat.Vector4] = 128;
            sizeInBits[(int) SurfaceFormat.HalfSingle] = 16;
            sizeInBits[(int) SurfaceFormat.HalfVector2] = 32;
            sizeInBits[(int) SurfaceFormat.HalfVector4] = 64;

            sizeInBits[(int) DepthFormat.Depth16] = 16;
            sizeInBits[(int) DepthFormat.Depth24Stencil8] = 32;

            // SurfaceFormat に無いもののみ登録。
            sizeInBits[(int) InputElementFormat.Byte4] = 32;
            sizeInBits[(int) InputElementFormat.Short2] = 32;
            sizeInBits[(int) InputElementFormat.Short4] = 64;
            sizeInBits[(int) InputElementFormat.NormalizedShort2] = 32;
            sizeInBits[(int) InputElementFormat.NormalizedShort4] = 64;
        }

        public static int SizeInBits(SurfaceFormat format)
        {
            return sizeInBits[(int) format];
        }

        public static int SizeInBits(DepthFormat format)
        {
            return sizeInBits[(int) format];
        }

        public static int SizeInBits(InputElementFormat format)
        {
            return sizeInBits[(int) format];
        }

        public static int SizeInBytes(SurfaceFormat format)
        {
            return SizeInBits(format) / 8;
        }

        public static int SizeInBytes(DepthFormat format)
        {
            return SizeInBits(format) / 8;
        }

        public static int SizeInBytes(InputElementFormat format)
        {
            return SizeInBits(format) / 8;
        }
    }
}
