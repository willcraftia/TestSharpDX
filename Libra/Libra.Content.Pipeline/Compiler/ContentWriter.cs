#region Using

using System;
using System.IO;

#endregion

namespace Libra.Content.Pipeline.Compiler
{
    public sealed class ContentWriter : BinaryWriter
    {
        ContentTypeWriterManager manager;

        // .NET 4.0 の BinaryWriter には引数 leaveOpen による制御がない。

        bool leaveOpen;

        internal ContentWriter(ContentTypeWriterManager manager, Stream stream, bool leaveOpen = false)
            : base(stream)
        {
            if (manager == null) throw new ArgumentNullException("manager");

            this.manager = manager;
            this.leaveOpen = leaveOpen;
        }

        public void WriteObject<T>(T value)
        {
            var typeWriter = manager[value.GetType()];
            typeWriter.Write(this, value);
        }

        public void WriteObject<T>(T value, ContentTypeWriter typeWriter)
        {
            if (typeWriter == null) throw new ArgumentNullException("typeWriter");
            typeWriter.Write(this, value);
        }

        protected override void Dispose(bool disposing)
        {
            // leaveOpen = false ならば元ストリームを閉じる。

            if (disposing && !leaveOpen)
                OutStream.Close();
        }
    }
}
