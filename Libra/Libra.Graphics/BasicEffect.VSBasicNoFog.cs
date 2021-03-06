﻿#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public partial class BasicEffect
    {
        static readonly byte[] VSBasicNoFog =
        {
             68,  88,  66,  67,  20,   3, 
            221, 125, 108, 131, 107, 138, 
             40,  72,  25, 121,   1,  41, 
            145, 196,   1,   0,   0,   0, 
            100,   2,   0,   0,   4,   0, 
              0,   0,  48,   0,   0,   0, 
            248,   0,   0,   0, 220,   1, 
              0,   0,  16,   2,   0,   0, 
             65, 111, 110,  57, 192,   0, 
              0,   0, 192,   0,   0,   0, 
              0,   2, 254, 255, 128,   0, 
              0,   0,  64,   0,   0,   0, 
              2,   0,  36,   0,   0,   0, 
             60,   0,   0,   0,  60,   0, 
              0,   0,  36,   0,   1,   0, 
             60,   0,   0,   0,   0,   0, 
              1,   0,   1,   0,   0,   0, 
              0,   0,   0,   0,  22,   0, 
              4,   0,   2,   0,   0,   0, 
              0,   0,   0,   0,   0,   0, 
              0,   2, 254, 255,  31,   0, 
              0,   2,   5,   0,   0, 128, 
              0,   0,  15, 144,   9,   0, 
              0,   3,   0,   0,   4, 192, 
              0,   0, 228, 144,   4,   0, 
            228, 160,   9,   0,   0,   3, 
              0,   0,   1, 128,   0,   0, 
            228, 144,   2,   0, 228, 160, 
              9,   0,   0,   3,   0,   0, 
              2, 128,   0,   0, 228, 144, 
              3,   0, 228, 160,   9,   0, 
              0,   3,   0,   0,   4, 128, 
              0,   0, 228, 144,   5,   0, 
            228, 160,   4,   0,   0,   4, 
              0,   0,   3, 192,   0,   0, 
            170, 128,   0,   0, 228, 160, 
              0,   0, 228, 128,   1,   0, 
              0,   2,   0,   0,   8, 192, 
              0,   0, 170, 128,   1,   0, 
              0,   2,   0,   0,  15, 224, 
              1,   0, 228, 160, 255, 255, 
              0,   0,  83,  72,  68,  82, 
            220,   0,   0,   0,  64,   0, 
              1,   0,  55,   0,   0,   0, 
             89,   0,   0,   4,  70, 142, 
             32,   0,   0,   0,   0,   0, 
             26,   0,   0,   0,  95,   0, 
              0,   3, 242,  16,  16,   0, 
              0,   0,   0,   0, 101,   0, 
              0,   3, 242,  32,  16,   0, 
              0,   0,   0,   0, 103,   0, 
              0,   4, 242,  32,  16,   0, 
              1,   0,   0,   0,   1,   0, 
              0,   0,  54,   0,   0,   6, 
            242,  32,  16,   0,   0,   0, 
              0,   0,  70, 142,  32,   0, 
              0,   0,   0,   0,   0,   0, 
              0,   0,  17,   0,   0,   8, 
             18,  32,  16,   0,   1,   0, 
              0,   0,  70,  30,  16,   0, 
              0,   0,   0,   0,  70, 142, 
             32,   0,   0,   0,   0,   0, 
             22,   0,   0,   0,  17,   0, 
              0,   8,  34,  32,  16,   0, 
              1,   0,   0,   0,  70,  30, 
             16,   0,   0,   0,   0,   0, 
             70, 142,  32,   0,   0,   0, 
              0,   0,  23,   0,   0,   0, 
             17,   0,   0,   8,  66,  32, 
             16,   0,   1,   0,   0,   0, 
             70,  30,  16,   0,   0,   0, 
              0,   0,  70, 142,  32,   0, 
              0,   0,   0,   0,  24,   0, 
              0,   0,  17,   0,   0,   8, 
            130,  32,  16,   0,   1,   0, 
              0,   0,  70,  30,  16,   0, 
              0,   0,   0,   0,  70, 142, 
             32,   0,   0,   0,   0,   0, 
             25,   0,   0,   0,  62,   0, 
              0,   1,  73,  83,  71,  78, 
             44,   0,   0,   0,   1,   0, 
              0,   0,   8,   0,   0,   0, 
             32,   0,   0,   0,   0,   0, 
              0,   0,   0,   0,   0,   0, 
              3,   0,   0,   0,   0,   0, 
              0,   0,  15,  15,   0,   0, 
             83,  86,  95,  80, 111, 115, 
            105, 116, 105, 111, 110,   0, 
             79,  83,  71,  78,  76,   0, 
              0,   0,   2,   0,   0,   0, 
              8,   0,   0,   0,  56,   0, 
              0,   0,   0,   0,   0,   0, 
              0,   0,   0,   0,   3,   0, 
              0,   0,   0,   0,   0,   0, 
             15,   0,   0,   0,  62,   0, 
              0,   0,   0,   0,   0,   0, 
              1,   0,   0,   0,   3,   0, 
              0,   0,   1,   0,   0,   0, 
             15,   0,   0,   0,  67,  79, 
             76,  79,  82,   0,  83,  86, 
             95,  80, 111, 115, 105, 116, 
            105, 111, 110,   0, 171, 171
        };
    }
}
