#region Using

using System;
using System.IO;

#endregion

namespace Libra.Content.Serialization
{
    public interface IContentSerializer
    {
        void Serialize(Stream stream, object content);

        object Deserialize(Stream stream);
    }
}
