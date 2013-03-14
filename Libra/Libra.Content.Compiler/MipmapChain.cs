#region Using

using System;
using System.Collections.ObjectModel;

#endregion

namespace Libra.Content.Compiler
{
    public sealed class MipmapChain : Collection<BitmapContent>
    {
        public static implicit operator MipmapChain(BitmapContent bitmap)
        {
            var result = new MipmapChain();
            result.Add(bitmap);
            return result;
        }
    }
}
