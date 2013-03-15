#region Using

using System;
using Libra.Content.Serialization;
using Libra.Content.Pipeline.Processors;

#endregion

namespace Libra.Content.Pipeline.Compiler
{
    public sealed class ContentCompilerFactory
    {
        public ContentSerializerCollection Serializers { get; private set; }

        public ContentProcessorTypeCollection ProcessorTypes { get; private set; }

        public ContentTypeWriterManager TypeWriters { get; private set; }

        public ContentCompilerFactory()
        {
            Serializers = new ContentSerializerCollection();
            ProcessorTypes = new ContentProcessorTypeCollection();
            TypeWriters = new ContentTypeWriterManager();
        }

        public ContentCompiler CreateCompiler()
        {
            return new ContentCompiler(this);
        }
    }
}
