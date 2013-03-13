#region Using

using System;
using System.IO;

#endregion

namespace Libra.Content.Compiler
{
    public sealed class ContentWriter : BinaryWriter
    {
        public ContentWriter(Stream stream)
            : base(stream)
        {
        }
    }
}
