#region Using

using System;
using Libra.Graphics;

#endregion

namespace Libra.Content.Xnb
{
    public static class XnbVertexElementFormatHelper
    {
        #region XnbVertexElementFormat

        enum XnbVertexElementFormat
        {
            Single              = 0,
            Vector2             = 1,
            Vector3             = 2,
            Vector4             = 3,
            Color               = 4,
            Byte4               = 5,
            Short2              = 6,
            Short4              = 7,
            NormalizedShort2    = 8,
            NormalizedShort4    = 9,
            HalfVector2         = 10,
            HalfVector4         = 11,
        }

        #endregion

        public static InputElementFormat ToInputElement(int xnbValue)
        {
            switch ((XnbVertexElementFormat) xnbValue)
            {
                case XnbVertexElementFormat.Single:
                    return InputElementFormat.Single;
                case XnbVertexElementFormat.Vector2:
                    return InputElementFormat.Vector2;
                case XnbVertexElementFormat.Vector3:
                    return InputElementFormat.Vector3;
                case XnbVertexElementFormat.Vector4:
                    return InputElementFormat.Vector4;
                case XnbVertexElementFormat.Color:
                    return InputElementFormat.Color;
                case XnbVertexElementFormat.Byte4:
                    return InputElementFormat.Byte4;
                case XnbVertexElementFormat.Short2:
                    return InputElementFormat.Short2;
                case XnbVertexElementFormat.Short4:
                    return InputElementFormat.Short4;
                case XnbVertexElementFormat.NormalizedShort2:
                    return InputElementFormat.NormalizedShort2;
                case XnbVertexElementFormat.NormalizedShort4:
                    return InputElementFormat.NormalizedShort4;
                case XnbVertexElementFormat.HalfVector2:
                    return InputElementFormat.HalfVector2;
                case XnbVertexElementFormat.HalfVector4:
                    return InputElementFormat.HalfVector4;
            }

            throw new NotSupportedException("Unknown vertex element format: " + xnbValue);
        }
    }
}
