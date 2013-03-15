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
            if (extension == null) throw new ArgumentNullException("extension");
            if (extension.Length == 0) throw new ArgumentException("Extension must be not empty.", "extension");

            extensions.Add(extension);
        }

        public ContentSerializerAttribute(params string[] extensions)
        {
            if (extensions == null) throw new ArgumentNullException("extensions");
            if (extensions.Length == 0) throw new ArgumentException("Extension array must be not empty.", "extensions");

            for (int i = 0; i < extensions.Length; i++)
            {
                var extension = extensions[i];
                if (string.IsNullOrEmpty(extension))
                    throw new ArgumentException(string.Format("extension[{0}] is null or empty", i), "extensions");

                this.extensions.Add(extension);
            }
        }
    }
}
