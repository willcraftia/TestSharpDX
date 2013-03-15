#region Using

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace Libra.Content.Pipeline.Build
{
    [DataContract]
    public sealed class ContentProjectDescription
    {
        [DataMember]
        public string OutputDirectory { get; set; }

        [DataMember]
        public bool DetectSerializers { get; set; }

        [DataMember]
        public bool DetectProcessors { get; set; }

        [DataMember]
        public List<ContentSerializerItem> Serializers { get; private set; }

        [DataMember]
        public List<ContentProcessorItem> Processors { get; private set; }

        [DataMember]
        public List<BuildTargetItem> BuildTargets { get; private set; }

        public ContentProjectDescription()
        {
            Serializers = new List<ContentSerializerItem>();
            Processors = new List<ContentProcessorItem>();
            BuildTargets = new List<BuildTargetItem>();
        }
    }
}
