#region Using

using System;

#endregion

namespace Libra.Content.Compiler
{
    public abstract class ContentTypeWriter<T> : IContentTypeWriter
    {
        public void Write(ContentWriter writer, object value)
        {
            Write(writer, (T) value);
        }

        protected abstract void Write(ContentWriter writer, T value);
    }
}
