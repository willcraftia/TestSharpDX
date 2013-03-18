#region Using

using System;
using System.IO;
using System.Runtime.Serialization.Json;

#endregion

namespace Libra.Content.Serialization
{
    public class JsonContentSerializer : IContentSerializer
    {
        DataContractJsonSerializer serializer;

        public Type TargetType { get; private set; }

        public JsonContentSerializer(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            TargetType = type;

            serializer = new DataContractJsonSerializer(type);
        }

        public virtual void Serialize(Stream stream, object content)
        {
            serializer.WriteObject(stream, content);
        }

        public virtual object Deserialize(Stream stream)
        {
            return serializer.ReadObject(stream);
        }
    }
}
