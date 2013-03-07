﻿#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public partial class BasicEffect
    {
        static readonly byte[] PSBasicVertexLightingTxNoFog =
        {
             68,  88,  66,  67,  81, 129, 
             64,  19, 174,  61,  32,  51, 
            160, 127, 244,  14, 156,  71, 
            151, 197,   1,   0,   0,   0, 
            100,   2,   0,   0,   4,   0, 
              0,   0,  48,   0,   0,   0, 
            216,   0,   0,   0, 200,   1, 
              0,   0,  48,   2,   0,   0, 
             65, 111, 110,  57, 160,   0, 
              0,   0, 160,   0,   0,   0, 
              0,   2, 255, 255, 120,   0, 
              0,   0,  40,   0,   0,   0, 
              0,   0,  40,   0,   0,   0, 
             40,   0,   0,   0,  40,   0, 
              1,   0,  36,   0,   0,   0, 
             40,   0,   0,   0,   0,   0, 
              0,   2, 255, 255,  31,   0, 
              0,   2,   0,   0,   0, 128, 
              0,   0,  15, 176,  31,   0, 
              0,   2,   0,   0,   0, 128, 
              1,   0,  15, 176,  31,   0, 
              0,   2,   0,   0,   0, 128, 
              2,   0,   3, 176,  31,   0, 
              0,   2,   0,   0,   0, 144, 
              0,   8,  15, 160,  66,   0, 
              0,   3,   0,   0,  15, 128, 
              2,   0, 228, 176,   0,   8, 
            228, 160,   5,   0,   0,   3, 
              0,   0,  15, 128,   0,   0, 
            228, 128,   0,   0, 228, 176, 
              4,   0,   0,   4,   0,   0, 
              7, 128,   1,   0, 228, 176, 
              0,   0, 255, 128,   0,   0, 
            228, 128,   1,   0,   0,   2, 
              0,   8,  15, 128,   0,   0, 
            228, 128, 255, 255,   0,   0, 
             83,  72,  68,  82, 232,   0, 
              0,   0,  64,   0,   0,   0, 
             58,   0,   0,   0,  89,   0, 
              0,   4,  70, 142,  32,   0, 
              0,   0,   0,   0,   1,   0, 
              0,   0,  90,   0,   0,   3, 
              0,  96,  16,   0,   0,   0, 
              0,   0,  88,  24,   0,   4, 
              0, 112,  16,   0,   0,   0, 
              0,   0,  85,  85,   0,   0, 
             98,  16,   0,   3, 242,  16, 
             16,   0,   0,   0,   0,   0, 
             98,  16,   0,   3, 114,  16, 
             16,   0,   1,   0,   0,   0, 
             98,  16,   0,   3,  50,  16, 
             16,   0,   2,   0,   0,   0, 
            101,   0,   0,   3, 242,  32, 
             16,   0,   0,   0,   0,   0, 
            104,   0,   0,   2,   1,   0, 
              0,   0,  69,   0,   0,   9, 
            242,   0,  16,   0,   0,   0, 
              0,   0,  70,  16,  16,   0, 
              2,   0,   0,   0,  70, 126, 
             16,   0,   0,   0,   0,   0, 
              0,  96,  16,   0,   0,   0, 
              0,   0,  56,   0,   0,   7, 
            242,   0,  16,   0,   0,   0, 
              0,   0,  70,  14,  16,   0, 
              0,   0,   0,   0,  70,  30, 
             16,   0,   0,   0,   0,   0, 
             50,   0,   0,   9, 114,  32, 
             16,   0,   0,   0,   0,   0, 
             70,  18,  16,   0,   1,   0, 
              0,   0, 246,  15,  16,   0, 
              0,   0,   0,   0,  70,   2, 
             16,   0,   0,   0,   0,   0, 
             54,   0,   0,   5, 130,  32, 
             16,   0,   0,   0,   0,   0, 
             58,   0,  16,   0,   0,   0, 
              0,   0,  62,   0,   0,   1, 
             73,  83,  71,  78,  96,   0, 
              0,   0,   3,   0,   0,   0, 
              8,   0,   0,   0,  80,   0, 
              0,   0,   0,   0,   0,   0, 
              0,   0,   0,   0,   3,   0, 
              0,   0,   0,   0,   0,   0, 
             15,  15,   0,   0,  80,   0, 
              0,   0,   1,   0,   0,   0, 
              0,   0,   0,   0,   3,   0, 
              0,   0,   1,   0,   0,   0, 
             15,   7,   0,   0,  86,   0, 
              0,   0,   0,   0,   0,   0, 
              0,   0,   0,   0,   3,   0, 
              0,   0,   2,   0,   0,   0, 
              3,   3,   0,   0,  67,  79, 
             76,  79,  82,   0,  84,  69, 
             88,  67,  79,  79,  82,  68, 
              0, 171,  79,  83,  71,  78, 
             44,   0,   0,   0,   1,   0, 
              0,   0,   8,   0,   0,   0, 
             32,   0,   0,   0,   0,   0, 
              0,   0,   0,   0,   0,   0, 
              3,   0,   0,   0,   0,   0, 
              0,   0,  15,   0,   0,   0, 
             83,  86,  95,  84,  97, 114, 
            103, 101, 116,   0, 171, 171
        };
    }
}