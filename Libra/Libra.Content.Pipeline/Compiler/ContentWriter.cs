#region Using

using System;
using System.IO;

#endregion

namespace Libra.Content.Pipeline.Compiler
{
    public sealed class ContentWriter : BinaryWriter
    {
        ContentCompiler compiler;

        internal ContentWriter(Stream stream, ContentCompiler compiler)
            : base(stream)
        {
            if (compiler == null) throw new ArgumentNullException("compiler");

            this.compiler = compiler;
        }

        public void WriteObject<T>(T value)
        {
            var typeWriter = compiler.GetTypeWriter(value.GetType());
            typeWriter.Write(this, value);
        }

        public void WriteObject<T>(T value, ContentTypeWriter typeWriter)
        {
            if (typeWriter == null) throw new ArgumentNullException("typeWriter");
            typeWriter.Write(this, value);
        }
    }
}
