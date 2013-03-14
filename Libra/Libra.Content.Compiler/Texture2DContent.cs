#region Using

using System;

#endregion

namespace Libra.Content.Compiler
{
    public sealed class Texture2DContent : TextureContent
    {
        public MipmapChain Mipmaps
        {
            get { return Faces[0]; }
            set { Faces[0] = value; }
        }

        public Texture2DContent()
            : base(new MipmapChainCollection())
        {
        }
    }
}
