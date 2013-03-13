﻿#region Using

using System;

#endregion

namespace Libra.Content.Compiler
{
    public abstract class TextureContent
    {
        public MipmapChainCollection Faces { get; private set; }

        protected TextureContent(MipmapChainCollection faces)
        {
            if (faces == null) throw new ArgumentNullException("faces");

            Faces = faces;
        }
    }
}
