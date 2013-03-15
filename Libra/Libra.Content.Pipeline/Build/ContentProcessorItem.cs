#region Using

using System;
using System.Runtime.Serialization;

#endregion

namespace Libra.Content.Pipeline.Build
{
    [DataContract]
    public sealed class ContentProcessorItem
    {
        [DataMember]
        public string AssemblyQualifiedName;
    }
}
