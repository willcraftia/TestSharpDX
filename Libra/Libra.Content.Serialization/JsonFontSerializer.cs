#region Using

using System;
using System.IO;

#endregion

namespace Libra.Content.Serialization
{
    [ContentSerializer]
    public sealed class JsonFontSerializer : JsonContentSerializer
    {
        public JsonFontSerializer()
            : base(typeof(FontDescription))
        {
        }

        public override object Deserialize(Stream stream)
        {
            var description = base.Deserialize(stream) as FontDescription;
            
            description.InitializeCharacters();
            
            return description;
        }
    }
}
