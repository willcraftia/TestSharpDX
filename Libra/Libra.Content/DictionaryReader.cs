#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Libra.Content
{
    public sealed class DictionaryReader<K, V> : ContentTypeReader<Dictionary<K, V>>
    {
        ContentTypeReader keyReader;

        ContentTypeReader valueReader;

        protected internal override void Initialize(ContentTypeReaderManager manager)
        {
            keyReader = manager[typeof(K)];
            valueReader = manager[typeof(V)];

            base.Initialize(manager);
        }

        protected internal override Dictionary<K, V> Read(ContentReader input, Dictionary<K, V> existingInstance)
        {
            var result = existingInstance ?? new Dictionary<K, V>();
            result.Clear();

            // TODO
            //
            // クラス型での null および多態性に関する考慮が必要。
            // 当面、null 禁止および多態性禁止として進める。

            var count = (int) input.ReadUInt32();

            for (int i = 0; i < count; i++)
            {
                var key = input.ReadObject<K>(keyReader);
                var value = input.ReadObject<V>(valueReader);
                
                result[key] = value;
            }

            return result;
        }
    }
}
