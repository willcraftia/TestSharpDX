﻿#region Using

using System;

#endregion

namespace Libra.Content.Compiler
{
    public interface IContentTypeWriter
    {
        void Write(ContentWriter output, object value);
    }
}
