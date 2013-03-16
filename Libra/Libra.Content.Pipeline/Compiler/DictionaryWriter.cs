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

        protected internal override void Initialize(ContentTypeWriterManager manager)
        {
            keyWriter = manager[typeof(K)];
            valueWriter = manager[typeof(V)];

            base.Initialize(manager);
        }

        protected internal override void Write(ContentWriter output, Dictionary<K, V> value)
        {
            output.Write(value.Count);

            // TODO
            //
            // クラス型での null および多態性に関する考慮が必要。
            // 当面、null 禁止および多態性禁止として進める。

            foreach (var entry in value)
            {
                output.WriteObject(entry.Key, keyWriter);
                output.WriteObject(entry.Value, valueWriter);
            }
        }
    }
}
