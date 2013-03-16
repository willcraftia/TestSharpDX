#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Libra.Content
{
    [ContentTypeReader]
    public sealed class ListReader<T> : ContentTypeReader<IList<T>>
    {
        ContentTypeReader itemReader;

        protected internal override void Initialize(ContentTypeReaderManager manager)
        {
            itemReader = manager[typeof(T)];

            base.Initialize(manager);
        }

        protected internal override IList<T> Read(ContentReader input, IList<T> existingInstance)
        {
            var result = existingInstance ?? new List<T>();
            result.Clear();

            // TODO
            //
            // クラス型での null および多態性に関する考慮が必要。
            // 当面、null 禁止および多態性禁止として進める。

            var count = (int) input.ReadUInt32();
            for (int i = 0; i < count; i++)
            {
                result.Add(input.ReadObject<T>(itemReader));
            }

            return result;
        }
    }
}
