#region Using

using System;

#endregion

namespace Libra.Content.Pipeline.Compiler
{
    public abstract class ContentTypeWriter
    {
        public Type TargetType { get; private set; }

        internal bool Initialized { get; private set; }

        protected ContentTypeWriter(Type targetType)
        {
            if (targetType == null) throw new ArgumentNullException("targetType");

            TargetType = targetType;
        }

        protected internal abstract void Write(ContentWriter output, Object value);

        protected internal virtual void Initialize(ContentTypeWriterManager manager)
        {
            Initialized = true;
        }
    }

    public abstract class ContentTypeWriter<T> : ContentTypeWriter
    {
        public ContentTypeWriter()
            : base(typeof(T))
        {
        }

        protected internal override void Write(ContentWriter output, object value)
        {
            Write(output, (T) value);
        }

        protected internal abstract void Write(ContentWriter output, T value);
    }
}
