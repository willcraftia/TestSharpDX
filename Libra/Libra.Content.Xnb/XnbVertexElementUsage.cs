﻿#region Using

using System;

#endregion

namespace Libra.Content.Xnb
{
    public enum XnbVertexElementUsage
    {
        Position            = 0,
        Color               = 1,
        TextureCoordinate   = 2,
        Normal              = 3,
        Binormal            = 4,
        Tangent             = 5,
        BlendIndices        = 6,
        BlendWeight         = 7,
        Depth               = 8,
        Fog                 = 9,
        PointSize           = 10,
        Sample              = 11,
        TessellateFactor    = 12,
    }
}
