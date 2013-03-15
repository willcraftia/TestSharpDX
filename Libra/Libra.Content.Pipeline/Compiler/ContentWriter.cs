#region Using

using System;
using System.IO;

#endregion

namespace Libra.Content.Pipeline.Compiler
{
    public sealed class ContentWriter : BinaryWriter
    {
        ContentTypeWriterManager manager;

        internal ContentWriter(Stream stream, ContentTypeWriterManager manager)
            : base(stream)
        {
            if (manager == null) throw new ArgumentNullException("manager");

            this.manager = manager;
        }

        public void WriteObject<T>(T value)
        {
            var typeWriter = manager.GetTypeWriter(value.GetType());
            typeWriter.Write(this, value);
        }

        public void WriteObject<T>(T value, ContentTypeWriter typeWriter)
        {
            if (typeWriter == null) throw new ArgumentNullException("typeWriter");
            typeWriter.Write(this, value);
        }
    }
}
