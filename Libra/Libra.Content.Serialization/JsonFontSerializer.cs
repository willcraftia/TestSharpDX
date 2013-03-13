#region Using

using System;
using System.IO;
using System.Runtime.Serialization.Json;

#endregion

namespace Libra.Content.Serialization
{
    public sealed class JsonFontSerializer : IContentSerializer
    {
        DataContractJsonSerializer serializer;

        public JsonFontSerializer()
        {
            serializer = new DataContractJsonSerializer(typeof(FontDescription));
        }

        public void Serialize(Stream stream, object content)
        {
            serializer.WriteObject(stream, content);
        }

        public object Deserialize(Stream stream)
        {
            var description = serializer.ReadObject(stream) as FontDescription;
            
            description.InitializeCharacters();
            
            return description;
        }
    }
}
