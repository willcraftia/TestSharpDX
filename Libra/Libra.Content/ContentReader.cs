#region Using

using System;
using System.IO;

#endregion

namespace Libra.Content
{
    public sealed class ContentReader : BinaryReader
    {
        public ContentReader(Stream stream)
            : base(stream)
        {
        }
    }
}
