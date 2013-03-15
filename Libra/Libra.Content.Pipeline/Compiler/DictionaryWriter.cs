#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Libra.Content.Pipeline.Compiler
{
    [ContentTypeWriter]
    public sealed class DictionaryWriter<K, V> : ContentTypeWriter<Dictionary<K, V>>
    {
        ContentTypeWriter keyWriter;

        ContentTypeWriter valueWriter;

        protected override void Initialize(ContentTypeWriterManager manager)
        {
            keyWriter = manager.GetTypeWriter(typeof(K));
            valueWriter = manager.GetTypeWriter(typeof(V));

            base.Initialize(manager);
        }

        protected internal override void Write(ContentWriter output, Dictionary<K, V> value)
        {
            output.Write(value.Count);

            foreach (var entry in value)
            {
                output.WriteObject(entry.Key, keyWriter);
                output.WriteObject(entry.Value, valueWriter);
            }
        }
    }
}
