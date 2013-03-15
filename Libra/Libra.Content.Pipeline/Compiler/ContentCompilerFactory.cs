#region Using

using System;
using Libra.Content.Serialization;
using Libra.Content.Pipeline.Processors;

#endregion

namespace Libra.Content.Pipeline.Compiler
{
    public sealed class ContentCompilerFactory
    {
        public ContentSerializerManager Serializers { get; private set; }

        public ContentProcessorManager ProcessorTypes { get; private set; }

        public ContentTypeWriterManager TypeWriters { get; private set; }

        public string SourceRootDirectory { get; set; }

        public string OutputRootDirectory { get; set; }

        public ContentCompilerFactory()
        {
            Serializers = new ContentSerializerManager();
            ProcessorTypes = new ContentProcessorManager();
            TypeWriters = new ContentTypeWriterManager();
        }

        public ContentCompilerFactory(AppDomain appDomain)
            : this()
        {
            FindAndAddAllFrom(appDomain);
        }

        public void FindAndAddAllFrom(AppDomain appDomain)
        {
            Serializers.FindAndAddFrom(appDomain);
            ProcessorTypes.FindAndAddFrom(appDomain);
            TypeWriters.FindAndAddFrom(appDomain);
        }

        public ContentCompiler CreateCompiler()
        {
            return new ContentCompiler(this);
        }
    }
}
