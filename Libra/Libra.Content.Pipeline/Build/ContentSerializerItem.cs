#region Using

using System;
using System.Runtime.Serialization;

#endregion

namespace Libra.Content.Pipeline.Build
{
    [DataContract]
    public sealed class ContentSerializerItem
    {
        [DataMember]
        public string AssemblyQualifiedName;

        [DataMember]
        public string[] Extensions;
    }
}
