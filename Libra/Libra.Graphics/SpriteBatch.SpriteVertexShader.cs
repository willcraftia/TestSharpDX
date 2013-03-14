﻿#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public partial class SpriteBatch
    {
        static readonly byte[] SpriteVertexShader =
        {
             68,  88,  66,  67,  53,  15, 
            108,  68,  10,  30,  79, 231, 
            153,   9,  61,  10, 198, 154, 
              8, 125,   1,   0,   0,   0, 
             60,   3,   0,   0,   4,   0, 
              0,   0,  48,   0,   0,   0, 
             28,   1,   0,   0,  84,   2, 
              0,   0, 200,   2,   0,   0, 
             65, 111, 110,  57, 228,   0, 
              0,   0, 228,   0,   0,   0, 
              0,   2, 254, 255, 176,   0, 
              0,   0,  52,   0,   0,   0, 
              1,   0,  36,   0,   0,   0, 
             48,   0,   0,   0,  48,   0, 
              0,   0,  36,   0,   1,   0, 
             48,   0,   0,   0,   0,   0, 
              4,   0,   1,   0,   0,   0, 
              0,   0,   0,   0,   0,   0, 
              0,   2, 254, 255,  31,   0, 
              0,   2,   5,   0,   0, 128, 
              0,   0,  15, 144,  31,   0, 
              0,   2,   5,   0,   1, 128, 
              1,   0,  15, 144,  31,   0, 
              0,   2,   5,   0,   2, 128, 
              2,   0,  15, 144,   5,   0, 
              0,   3,   0,   0,  15, 128, 
              2,   0,  85, 144,   2,   0, 
            228, 160,   4,   0,   0,   4, 
              0,   0,  15, 128,   2,   0, 
              0, 144,   1,   0, 228, 160, 
              0,   0, 228, 128,   4,   0, 
              0,   4,   0,   0,  15, 128, 
              2,   0, 170, 144,   3,   0, 
            228, 160,   0,   0, 228, 128, 
              4,   0,   0,   4,   0,   0, 
             15, 128,   2,   0, 255, 144, 
              4,   0, 228, 160,   0,   0, 
            228, 128,   4,   0,   0,   4, 
              0,   0,   3, 192,   0,   0, 
            255, 128,   0,   0, 228, 160, 
              0,   0, 228, 128,   1,   0, 
              0,   2,   0,   0,  12, 192, 
              0,   0, 228, 128,   1,   0, 
              0,   2,   0,   0,  15, 224, 
              0,   0, 228, 144,   1,   0, 
              0,   2,   1,   0,   3, 224, 
              1,   0, 228, 144, 255, 255, 
              0,   0,  83,  72,  68,  82, 
             48,   1,   0,   0,  64,   0, 
              1,   0,  76,   0,   0,   0, 
             89,   0,   0,   4,  70, 142, 
             32,   0,   0,   0,   0,   0, 
              4,   0,   0,   0,  95,   0, 
              0,   3, 242,  16,  16,   0, 
              0,   0,   0,   0,  95,   0, 
              0,   3,  50,  16,  16,   0, 
              1,   0,   0,   0,  95,   0, 
              0,   3, 242,  16,  16,   0, 
              2,   0,   0,   0, 101,   0, 
              0,   3, 242,  32,  16,   0, 
              0,   0,   0,   0, 101,   0, 
              0,   3,  50,  32,  16,   0, 
              1,   0,   0,   0, 103,   0, 
              0,   4, 242,  32,  16,   0, 
              2,   0,   0,   0,   1,   0, 
              0,   0, 104,   0,   0,   2, 
              1,   0,   0,   0,  54,   0, 
              0,   5, 242,  32,  16,   0, 
              0,   0,   0,   0,  70,  30, 
             16,   0,   0,   0,   0,   0, 
             54,   0,   0,   5,  50,  32, 
             16,   0,   1,   0,   0,   0, 
             70,  16,  16,   0,   1,   0, 
              0,   0,  56,   0,   0,   8, 
            242,   0,  16,   0,   0,   0, 
              0,   0,  86,  21,  16,   0, 
              2,   0,   0,   0,  70, 142, 
             32,   0,   0,   0,   0,   0, 
              1,   0,   0,   0,  50,   0, 
              0,  10, 242,   0,  16,   0, 
              0,   0,   0,   0,   6,  16, 
             16,   0,   2,   0,   0,   0, 
             70, 142,  32,   0,   0,   0, 
              0,   0,   0,   0,   0,   0, 
             70,  14,  16,   0,   0,   0, 
              0,   0,  50,   0,   0,  10, 
            242,   0,  16,   0,   0,   0, 
              0,   0, 166,  26,  16,   0, 
              2,   0,   0,   0,  70, 142, 
             32,   0,   0,   0,   0,   0, 
              2,   0,   0,   0,  70,  14, 
             16,   0,   0,   0,   0,   0, 
             50,   0,   0,  10, 242,  32, 
             16,   0,   2,   0,   0,   0, 
            246,  31,  16,   0,   2,   0, 
              0,   0,  70, 142,  32,   0, 
              0,   0,   0,   0,   3,   0, 
              0,   0,  70,  14,  16,   0, 
              0,   0,   0,   0,  62,   0, 
              0,   1,  73,  83,  71,  78, 
            108,   0,   0,   0,   3,   0, 
              0,   0,   8,   0,   0,   0, 
             80,   0,   0,   0,   0,   0, 
              0,   0,   0,   0,   0,   0, 
              3,   0,   0,   0,   0,   0, 
              0,   0,  15,  15,   0,   0, 
             86,   0,   0,   0,   0,   0, 
              0,   0,   0,   0,   0,   0, 
              3,   0,   0,   0,   1,   0, 
              0,   0,   3,   3,   0,   0, 
             95,   0,   0,   0,   0,   0, 
              0,   0,   0,   0,   0,   0, 
              3,   0,   0,   0,   2,   0, 
              0,   0,  15,  15,   0,   0, 
             67,  79,  76,  79,  82,   0, 
             84,  69,  88,  67,  79,  79, 
             82,  68,   0,  83,  86,  95, 
             80, 111, 115, 105, 116, 105, 
            111, 110,   0, 171,  79,  83, 
             71,  78, 108,   0,   0,   0, 
              3,   0,   0,   0,   8,   0, 
              0,   0,  80,   0,   0,   0, 
              0,   0,   0,   0,   0,   0, 
              0,   0,   3,   0,   0,   0, 
              0,   0,   0,   0,  15,   0, 
              0,   0,  86,   0,   0,   0, 
              0,   0,   0,   0,   0,   0, 
              0,   0,   3,   0,   0,   0, 
              1,   0,   0,   0,   3,  12, 
              0,   0,  95,   0,   0,   0, 
              0,   0,   0,   0,   1,   0, 
              0,   0,   3,   0,   0,   0, 
              2,   0,   0,   0,  15,   0, 
              0,   0,  67,  79,  76,  79, 
             82,   0,  84,  69,  88,  67, 
             79,  79,  82,  68,   0,  83, 
             86,  95,  80, 111, 115, 105, 
            116, 105, 111, 110,   0, 171
        };
    }
}