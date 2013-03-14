#region Using

using System;
using System.Collections.Generic;
using System.IO;

#endregion

namespace Libra.Content.Compiler
{
    public sealed class ContentWriter : BinaryWriter
    {
        public Dictionary<Type, IContentTypeWriter> TypeWriters { get; private set; }

        public ContentWriter(Stream stream)
            : base(stream)
        {
            TypeWriters = new Dictionary<Type, IContentTypeWriter>();
        }

        public void WriteObject<T>(T value)
        {
            var type = typeof(T);

            IContentTypeWriter typeWriter;
            if (!TypeWriters.TryGetValue(type, out typeWriter))
            {
                if (type.IsGenericType)
                {
                    var genericTypeDefinition = type.GetGenericTypeDefinition();
                    TypeWriters.TryGetValue(genericTypeDefinition, out typeWriter);
                }
            }

            if (typeWriter == null)
            {
                throw new InvalidOperationException("IContentTypeWriter not found: " + type);
            }

            typeWriter.Write(this, value);
        }
    }
}
