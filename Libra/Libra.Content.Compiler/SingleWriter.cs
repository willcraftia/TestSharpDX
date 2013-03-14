﻿#region Using

using System;

#endregion

namespace Libra.Content.Compiler
{
    public sealed class SingleWriter : ContentTypeWriter<float>
    {
        protected internal override void Write(ContentWriter output, float value)
        {
            output.Write(value);
        }
    }
}
