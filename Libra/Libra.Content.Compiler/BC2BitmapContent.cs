#region Using

using System;
using Libra.Graphics;

#endregion

namespace Libra.Content.Compiler
{
    public sealed class BC2BitmapContent : BCBitmapContent
    {
        public BC2BitmapContent(int width, int height)
            : base(width, height, 4)
        {
        }

        public override bool TryGetFormat(out SurfaceFormat format)
        {
            format = SurfaceFormat.BC2;
            return true;
        }
    }
}
