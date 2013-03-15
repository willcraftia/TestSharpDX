#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Libra.Content.Pipeline.Compiler
{
    [ContentTypeWriter]
    public sealed class ListWriter<T> : ContentTypeWriter<IList<T>>
    {
        ContentTypeWriter itemWriter;

        protected override void Initialize(ContentTypeWriterManager manager)
        {
            itemWriter = manager[typeof(T)];

            base.Initialize(manager);
        }

        protected internal override void Write(ContentWriter output, IList<T> value)
        {
            output.Write((uint) value.Count);

            foreach (var item in value)
            {
                output.WriteObject(item, itemWriter);
            }
        }
    }
}
