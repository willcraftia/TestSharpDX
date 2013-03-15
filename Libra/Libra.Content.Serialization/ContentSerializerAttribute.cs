#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Libra.Content.Serialization
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ContentSerializerAttribute : Attribute
    {
        List<string> extensions = new List<string>();

        public string[] Extensions
        {
            get { return extensions.ToArray(); }
        }

        public ContentSerializerAttribute(string extension)
        {
            extensions.Add(extension);
        }

        public ContentSerializerAttribute(params string[] extensions)
        {
            this.extensions.AddRange(extensions);
        }
    }
}
