#region Using

using System;
using Libra.Graphics;

#endregion

namespace Libra.Content.Xnb
{
    public static class XnbSurfaceFormatHelper
    {
        #region XnbSurfaceFormat

        enum XnbSurfaceFormat
        {
            Color           = 0,
            Bgr565          = 1,
            Bgra5551        = 2,
            Bgra4444        = 3,
            Dxt1            = 4,
            Dxt3            = 5,
            Dxt5            = 6,
            NormalizedByte2 = 7,
            NormalizedByte4 = 8,
            Rgba1010102     = 9,
            Rg32            = 10,
            Rgba64          = 11,
            Alpha8          = 12,
            Single          = 13,
            Vector2         = 14,
            Vector4         = 15,
            HalfSingle      = 16,
            HalfVector2     = 17,
            HalfVector4     = 18,
            HdrBlendable    = 19,
        }

        #endregion

        public static SurfaceFormat ToSurfaceFormat(int xnbValue)
        {
            switch ((XnbSurfaceFormat) xnbValue)
            {
                case XnbSurfaceFormat.Color:
                    return SurfaceFormat.Color;
                case XnbSurfaceFormat.Bgr565:
                    return SurfaceFormat.Bgr565;
                case XnbSurfaceFormat.Bgra5551:
                    return SurfaceFormat.Bgra5551;
                case XnbSurfaceFormat.Bgra4444:
                    return SurfaceFormat.Bgra4444;
                case XnbSurfaceFormat.Dxt1:
                    return SurfaceFormat.BC1;
                case XnbSurfaceFormat.Dxt3:
                    return SurfaceFormat.BC2;
                case XnbSurfaceFormat.Dxt5:
                    return SurfaceFormat.BC3;
                case XnbSurfaceFormat.NormalizedByte2:
                    return SurfaceFormat.NormalizedByte2;
                case XnbSurfaceFormat.NormalizedByte4:
                    return SurfaceFormat.NormalizedByte4;
                case XnbSurfaceFormat.Rgba1010102:
                    return SurfaceFormat.Rgba1010102;
                case XnbSurfaceFormat.Rg32:
                    return SurfaceFormat.Rg32;
                case XnbSurfaceFormat.Alpha8:
                    return SurfaceFormat.Alpha8;
                case XnbSurfaceFormat.Single:
                    return SurfaceFormat.Single;
                case XnbSurfaceFormat.Vector2:
                    return SurfaceFormat.Vector2;
                case XnbSurfaceFormat.Vector4:
                    return SurfaceFormat.Vector4;
                case XnbSurfaceFormat.HalfSingle:
                    return SurfaceFormat.HalfSingle;
                case XnbSurfaceFormat.HalfVector2:
                    return SurfaceFormat.HalfVector2;
                case XnbSurfaceFormat.HalfVector4:
                    return SurfaceFormat.HalfVector4;
                case XnbSurfaceFormat.HdrBlendable:
                    throw new NotSupportedException("HdrBlendable not supported.");
            }

            throw new NotSupportedException("Unknown surface format: " + xnbValue);
        }
    }
}
