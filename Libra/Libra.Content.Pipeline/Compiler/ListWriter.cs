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

        protected internal override void Initialize(ContentTypeWriterManager manager)
        {
            itemWriter = manager[typeof(T)];

            base.Initialize(manager);
        }

        protected internal override void Write(ContentWriter output, IList<T> value)
        {
            output.Write((uint) value.Count);

            // TODO
            //
            // クラス型での null および多態性に関する考慮が必要。
            // 当面、null 禁止および多態性禁止として進める。

            foreach (var item in value)
            {
                output.WriteObject(item, itemWriter);
            }
        }
    }
}
