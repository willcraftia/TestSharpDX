﻿#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public partial class BasicEffect
    {
        static readonly byte[] VSBasicOneLightVc =
        {
             68,  88,  66,  67, 157, 229, 
             30, 248, 170,  66,  14, 167, 
            246, 135, 241,   9, 201, 147, 
            148, 127,   1,   0,   0,   0, 
            156,   8,   0,   0,   4,   0, 
              0,   0,  48,   0,   0,   0, 
            240,   2,   0,   0, 188,   7, 
              0,   0,  48,   8,   0,   0, 
             65, 111, 110,  57, 184,   2, 
              0,   0, 184,   2,   0,   0, 
              0,   2, 254, 255,  72,   2, 
              0,   0, 112,   0,   0,   0, 
              6,   0,  36,   0,   0,   0, 
            108,   0,   0,   0, 108,   0, 
              0,   0,  36,   0,   1,   0, 
            108,   0,   0,   0,   0,   0, 
              4,   0,   1,   0,   0,   0, 
              0,   0,   0,   0,   6,   0, 
              1,   0,   5,   0,   0,   0, 
              0,   0,   0,   0,   9,   0, 
              1,   0,   6,   0,   0,   0, 
              0,   0,   0,   0,  12,   0, 
              1,   0,   7,   0,   0,   0, 
              0,   0,   0,   0,  14,   0, 
              4,   0,   8,   0,   0,   0, 
              0,   0,   0,   0,  19,   0, 
              7,   0,  12,   0,   0,   0, 
              0,   0,   0,   0,   0,   0, 
              0,   2, 254, 255,  81,   0, 
              0,   5,  19,   0,  15, 160, 
              0,   0,   0,   0,   0,   0, 
            128,  63,   0,   0,   0,   0, 
              0,   0,   0,   0,  31,   0, 
              0,   2,   5,   0,   0, 128, 
              0,   0,  15, 144,  31,   0, 
              0,   2,   5,   0,   1, 128, 
              1,   0,  15, 144,  31,   0, 
              0,   2,   5,   0,   2, 128, 
              2,   0,  15, 144,   9,   0, 
              0,   3,   0,   0,   1, 128, 
              0,   0, 228, 144,   9,   0, 
            228, 160,   9,   0,   0,   3, 
              0,   0,   2, 128,   0,   0, 
            228, 144,  10,   0, 228, 160, 
              9,   0,   0,   3,   0,   0, 
              4, 128,   0,   0, 228, 144, 
             11,   0, 228, 160,   2,   0, 
              0,   3,   0,   0,   7, 128, 
              0,   0, 228, 129,   7,   0, 
            228, 160,  36,   0,   0,   2, 
              1,   0,   7, 128,   0,   0, 
            228, 128,   2,   0,   0,   3, 
              0,   0,   7, 128,   1,   0, 
            228, 128,   4,   0, 228, 161, 
             36,   0,   0,   2,   1,   0, 
              7, 128,   0,   0, 228, 128, 
              8,   0,   0,   3,   0,   0, 
              1, 128,   1,   0, 228, 144, 
             12,   0, 228, 160,   8,   0, 
              0,   3,   0,   0,   2, 128, 
              1,   0, 228, 144,  13,   0, 
            228, 160,   8,   0,   0,   3, 
              0,   0,   4, 128,   1,   0, 
            228, 144,  14,   0, 228, 160, 
             36,   0,   0,   2,   2,   0, 
              7, 128,   0,   0, 228, 128, 
              8,   0,   0,   3,   0,   0, 
              1, 128,   1,   0, 228, 128, 
              2,   0, 228, 128,   8,   0, 
              0,   3,   0,   0,   2, 128, 
              4,   0, 228, 161,   2,   0, 
            228, 128,  11,   0,   0,   3, 
              0,   0,   1, 128,   0,   0, 
              0, 128,  19,   0,   0, 160, 
             13,   0,   0,   3,   0,   0, 
              4, 128,   0,   0,  85, 128, 
             19,   0,   0, 160,   5,   0, 
              0,   3,   0,   0,   3, 128, 
              0,   0, 230, 128,   0,   0, 
            232, 128,  32,   0,   0,   3, 
              1,   0,   1, 128,   0,   0, 
              0, 128,   3,   0, 255, 160, 
              5,   0,   0,   3,   0,   0, 
             13, 128,   1,   0,   0, 128, 
              6,   0, 148, 160,   5,   0, 
              0,   3,   1,   0,   7, 224, 
              0,   0, 248, 128,   3,   0, 
            228, 160,   5,   0,   0,   3, 
              0,   0,   7, 128,   0,   0, 
             85, 128,   5,   0, 228, 160, 
              1,   0,   0,   2,   1,   0, 
              7, 128,   1,   0, 228, 160, 
              4,   0,   0,   4,   0,   0, 
              7, 128,   0,   0, 228, 128, 
              1,   0, 228, 128,   2,   0, 
            228, 160,   5,   0,   0,   3, 
              0,   0,   7, 224,   0,   0, 
            228, 128,   2,   0, 228, 144, 
              9,   0,   0,   3,   0,   0, 
              4, 192,   0,   0, 228, 144, 
             17,   0, 228, 160,   9,   0, 
              0,   3,   0,   0,   1, 128, 
              0,   0, 228, 144,   8,   0, 
            228, 160,  11,   0,   0,   3, 
              0,   0,   1, 128,   0,   0, 
              0, 128,  19,   0,   0, 160, 
             10,   0,   0,   3,   1,   0, 
              8, 224,   0,   0,   0, 128, 
             19,   0,  85, 160,   5,   0, 
              0,   3,   0,   0,   8, 224, 
              2,   0, 255, 144,   1,   0, 
            255, 160,   9,   0,   0,   3, 
              0,   0,   1, 128,   0,   0, 
            228, 144,  15,   0, 228, 160, 
              9,   0,   0,   3,   0,   0, 
              2, 128,   0,   0, 228, 144, 
             16,   0, 228, 160,   9,   0, 
              0,   3,   0,   0,   4, 128, 
              0,   0, 228, 144,  18,   0, 
            228, 160,   4,   0,   0,   4, 
              0,   0,   3, 192,   0,   0, 
            170, 128,   0,   0, 228, 160, 
              0,   0, 228, 128,   1,   0, 
              0,   2,   0,   0,   8, 192, 
              0,   0, 170, 128, 255, 255, 
              0,   0,  83,  72,  68,  82, 
            196,   4,   0,   0,  64,   0, 
              1,   0,  49,   1,   0,   0, 
             89,   0,   0,   4,  70, 142, 
             32,   0,   0,   0,   0,   0, 
             26,   0,   0,   0,  95,   0, 
              0,   3, 242,  16,  16,   0, 
              0,   0,   0,   0,  95,   0, 
              0,   3, 114,  16,  16,   0, 
              1,   0,   0,   0,  95,   0, 
              0,   3, 242,  16,  16,   0, 
              2,   0,   0,   0, 101,   0, 
              0,   3, 242,  32,  16,   0, 
              0,   0,   0,   0, 101,   0, 
              0,   3, 242,  32,  16,   0, 
              1,   0,   0,   0, 103,   0, 
              0,   4, 242,  32,  16,   0, 
              2,   0,   0,   0,   1,   0, 
              0,   0, 104,   0,   0,   2, 
              3,   0,   0,   0,  16,   0, 
              0,   8,  18,   0,  16,   0, 
              0,   0,   0,   0,  70,  18, 
             16,   0,   1,   0,   0,   0, 
             70, 130,  32,   0,   0,   0, 
              0,   0,  19,   0,   0,   0, 
             16,   0,   0,   8,  34,   0, 
             16,   0,   0,   0,   0,   0, 
             70,  18,  16,   0,   1,   0, 
              0,   0,  70, 130,  32,   0, 
              0,   0,   0,   0,  20,   0, 
              0,   0,  16,   0,   0,   8, 
             66,   0,  16,   0,   0,   0, 
              0,   0,  70,  18,  16,   0, 
              1,   0,   0,   0,  70, 130, 
             32,   0,   0,   0,   0,   0, 
             21,   0,   0,   0,  16,   0, 
              0,   7, 130,   0,  16,   0, 
              0,   0,   0,   0,  70,   2, 
             16,   0,   0,   0,   0,   0, 
             70,   2,  16,   0,   0,   0, 
              0,   0,  68,   0,   0,   5, 
            130,   0,  16,   0,   0,   0, 
              0,   0,  58,   0,  16,   0, 
              0,   0,   0,   0,  56,   0, 
              0,   7, 114,   0,  16,   0, 
              0,   0,   0,   0, 246,  15, 
             16,   0,   0,   0,   0,   0, 
             70,   2,  16,   0,   0,   0, 
              0,   0,  16,   0,   0,   9, 
            130,   0,  16,   0,   0,   0, 
              0,   0,  70, 130,  32, 128, 
             65,   0,   0,   0,   0,   0, 
              0,   0,   3,   0,   0,   0, 
             70,   2,  16,   0,   0,   0, 
              0,   0,  29,   0,   0,   7, 
             18,   0,  16,   0,   1,   0, 
              0,   0,  58,   0,  16,   0, 
              0,   0,   0,   0,   1,  64, 
              0,   0,   0,   0,   0,   0, 
              1,   0,   0,   7,  18,   0, 
             16,   0,   1,   0,   0,   0, 
             10,   0,  16,   0,   1,   0, 
              0,   0,   1,  64,   0,   0, 
              0,   0, 128,  63,  56,   0, 
              0,   7, 130,   0,  16,   0, 
              0,   0,   0,   0,  58,   0, 
             16,   0,   0,   0,   0,   0, 
             10,   0,  16,   0,   1,   0, 
              0,   0,  56,   0,   0,   8, 
            226,   0,  16,   0,   1,   0, 
              0,   0, 246,  15,  16,   0, 
              0,   0,   0,   0,   6, 137, 
             32,   0,   0,   0,   0,   0, 
              6,   0,   0,   0,  50,   0, 
              0,  11, 226,   0,  16,   0, 
              1,   0,   0,   0,  86,  14, 
             16,   0,   1,   0,   0,   0, 
              6, 137,  32,   0,   0,   0, 
              0,   0,   0,   0,   0,   0, 
              6, 137,  32,   0,   0,   0, 
              0,   0,   1,   0,   0,   0, 
             56,   0,   0,   7, 114,  32, 
             16,   0,   0,   0,   0,   0, 
            150,   7,  16,   0,   1,   0, 
              0,   0,  70,  18,  16,   0, 
              2,   0,   0,   0,  56,   0, 
              0,   8, 130,  32,  16,   0, 
              0,   0,   0,   0,  58,  16, 
             16,   0,   2,   0,   0,   0, 
             58, 128,  32,   0,   0,   0, 
              0,   0,   0,   0,   0,   0, 
             17,   0,   0,   8,  18,   0, 
             16,   0,   2,   0,   0,   0, 
             70,  30,  16,   0,   0,   0, 
              0,   0,  70, 142,  32,   0, 
              0,   0,   0,   0,  15,   0, 
              0,   0,  17,   0,   0,   8, 
             34,   0,  16,   0,   2,   0, 
              0,   0,  70,  30,  16,   0, 
              0,   0,   0,   0,  70, 142, 
             32,   0,   0,   0,   0,   0, 
             16,   0,   0,   0,  17,   0, 
              0,   8,  66,   0,  16,   0, 
              2,   0,   0,   0,  70,  30, 
             16,   0,   0,   0,   0,   0, 
             70, 142,  32,   0,   0,   0, 
              0,   0,  17,   0,   0,   0, 
              0,   0,   0,   9, 226,   0, 
             16,   0,   1,   0,   0,   0, 
              6,   9,  16, 128,  65,   0, 
              0,   0,   2,   0,   0,   0, 
              6, 137,  32,   0,   0,   0, 
              0,   0,  12,   0,   0,   0, 
             16,   0,   0,   7, 130,   0, 
             16,   0,   0,   0,   0,   0, 
            150,   7,  16,   0,   1,   0, 
              0,   0, 150,   7,  16,   0, 
              1,   0,   0,   0,  68,   0, 
              0,   5, 130,   0,  16,   0, 
              0,   0,   0,   0,  58,   0, 
             16,   0,   0,   0,   0,   0, 
             50,   0,   0,  11, 226,   0, 
             16,   0,   1,   0,   0,   0, 
             86,  14,  16,   0,   1,   0, 
              0,   0, 246,  15,  16,   0, 
              0,   0,   0,   0,   6, 137, 
             32, 128,  65,   0,   0,   0, 
              0,   0,   0,   0,   3,   0, 
              0,   0,  16,   0,   0,   7, 
            130,   0,  16,   0,   0,   0, 
              0,   0, 150,   7,  16,   0, 
              1,   0,   0,   0, 150,   7, 
             16,   0,   1,   0,   0,   0, 
             68,   0,   0,   5, 130,   0, 
             16,   0,   0,   0,   0,   0, 
             58,   0,  16,   0,   0,   0, 
              0,   0,  56,   0,   0,   7, 
            226,   0,  16,   0,   1,   0, 
              0,   0, 246,  15,  16,   0, 
              0,   0,   0,   0,  86,  14, 
             16,   0,   1,   0,   0,   0, 
             16,   0,   0,   7,  18,   0, 
             16,   0,   0,   0,   0,   0, 
            150,   7,  16,   0,   1,   0, 
              0,   0,  70,   2,  16,   0, 
              0,   0,   0,   0,  52,   0, 
              0,   7,  18,   0,  16,   0, 
              0,   0,   0,   0,  10,   0, 
             16,   0,   0,   0,   0,   0, 
              1,  64,   0,   0,   0,   0, 
              0,   0,  56,   0,   0,   7, 
             18,   0,  16,   0,   0,   0, 
              0,   0,  10,   0,  16,   0, 
              1,   0,   0,   0,  10,   0, 
             16,   0,   0,   0,   0,   0, 
             47,   0,   0,   5,  18,   0, 
             16,   0,   0,   0,   0,   0, 
             10,   0,  16,   0,   0,   0, 
              0,   0,  56,   0,   0,   8, 
             18,   0,  16,   0,   0,   0, 
              0,   0,  10,   0,  16,   0, 
              0,   0,   0,   0,  58, 128, 
             32,   0,   0,   0,   0,   0, 
              2,   0,   0,   0,  25,   0, 
              0,   5,  18,   0,  16,   0, 
              0,   0,   0,   0,  10,   0, 
             16,   0,   0,   0,   0,   0, 
             56,   0,   0,   8, 114,   0, 
             16,   0,   0,   0,   0,   0, 
              6,   0,  16,   0,   0,   0, 
              0,   0,  70, 130,  32,   0, 
              0,   0,   0,   0,   9,   0, 
              0,   0,  56,   0,   0,   8, 
            114,  32,  16,   0,   1,   0, 
              0,   0,  70,   2,  16,   0, 
              0,   0,   0,   0,  70, 130, 
             32,   0,   0,   0,   0,   0, 
              2,   0,   0,   0,  17,  32, 
              0,   8, 130,  32,  16,   0, 
              1,   0,   0,   0,  70,  30, 
             16,   0,   0,   0,   0,   0, 
             70, 142,  32,   0,   0,   0, 
              0,   0,  14,   0,   0,   0, 
             17,   0,   0,   8,  18,  32, 
             16,   0,   2,   0,   0,   0, 
             70,  30,  16,   0,   0,   0, 
              0,   0,  70, 142,  32,   0, 
              0,   0,   0,   0,  22,   0, 
              0,   0,  17,   0,   0,   8, 
             34,  32,  16,   0,   2,   0, 
              0,   0,  70,  30,  16,   0, 
              0,   0,   0,   0,  70, 142, 
             32,   0,   0,   0,   0,   0, 
             23,   0,   0,   0,  17,   0, 
              0,   8,  66,  32,  16,   0, 
              2,   0,   0,   0,  70,  30, 
             16,   0,   0,   0,   0,   0, 
             70, 142,  32,   0,   0,   0, 
              0,   0,  24,   0,   0,   0, 
             17,   0,   0,   8, 130,  32, 
             16,   0,   2,   0,   0,   0, 
             70,  30,  16,   0,   0,   0, 
              0,   0,  70, 142,  32,   0, 
              0,   0,   0,   0,  25,   0, 
              0,   0,  62,   0,   0,   1, 
             73,  83,  71,  78, 108,   0, 
              0,   0,   3,   0,   0,   0, 
              8,   0,   0,   0,  80,   0, 
              0,   0,   0,   0,   0,   0, 
              0,   0,   0,   0,   3,   0, 
              0,   0,   0,   0,   0,   0, 
             15,  15,   0,   0,  92,   0, 
              0,   0,   0,   0,   0,   0, 
              0,   0,   0,   0,   3,   0, 
              0,   0,   1,   0,   0,   0, 
              7,   7,   0,   0,  99,   0, 
              0,   0,   0,   0,   0,   0, 
              0,   0,   0,   0,   3,   0, 
              0,   0,   2,   0,   0,   0, 
             15,  15,   0,   0,  83,  86, 
             95,  80, 111, 115, 105, 116, 
            105, 111, 110,   0,  78,  79, 
             82,  77,  65,  76,   0,  67, 
             79,  76,  79,  82,   0, 171, 
            171, 171,  79,  83,  71,  78, 
            100,   0,   0,   0,   3,   0, 
              0,   0,   8,   0,   0,   0, 
             80,   0,   0,   0,   0,   0, 
              0,   0,   0,   0,   0,   0, 
              3,   0,   0,   0,   0,   0, 
              0,   0,  15,   0,   0,   0, 
             80,   0,   0,   0,   1,   0, 
              0,   0,   0,   0,   0,   0, 
              3,   0,   0,   0,   1,   0, 
              0,   0,  15,   0,   0,   0, 
             86,   0,   0,   0,   0,   0, 
              0,   0,   1,   0,   0,   0, 
              3,   0,   0,   0,   2,   0, 
              0,   0,  15,   0,   0,   0, 
             67,  79,  76,  79,  82,   0, 
             83,  86,  95,  80, 111, 115, 
            105, 116, 105, 111, 110,   0, 
            171, 171
        };
    }
}
