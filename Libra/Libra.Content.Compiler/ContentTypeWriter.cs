#region Using

using System;

#endregion

namespace Libra.Content.Compiler
{
    public abstract class ContentTypeWriter<T> : IContentTypeWriter
    {
        public void Write(ContentWriter output, object value)
        {
            Write(output, (T) value);
        }

        protected abstract void Write(ContentWriter output, T value);
    }
}
