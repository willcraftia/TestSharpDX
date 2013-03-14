#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Libra.Content.Compiler
{
    public sealed class ContentCompiler
    {
        public Dictionary<Type, ContentTypeWriter> TypeWriters { get; private set; }

        public ContentCompiler()
        {
            TypeWriters = new Dictionary<Type, ContentTypeWriter>();
        }

        public ContentTypeWriter GetTypeWriter(Type type)
        {
            ContentTypeWriter typeWriter;
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

            return typeWriter;
        }
    }
}
