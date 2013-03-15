#region Using

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace Libra.Content.Pipeline.Build
{
    [DataContract]
    public sealed class BuildTargetItem
    {
        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public string ProcessorName { get; set; }

        [DataMember]
        public Dictionary<string, string> ProcessorProperties { get; private set; }

        public BuildTargetItem()
        {
            ProcessorProperties = new Dictionary<string, string>();
        }
    }
}
