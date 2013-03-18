#region Using

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace Libra.Content.Serialization
{
    [DataContract]
    public sealed class CharacterRegion
    {
        [DataMember]
        public char Start { get; set; }

        [DataMember]
        public char End { get; set; }

        [IgnoreDataMember]
        public IEnumerable<char> Characters
        {
            get
            {
                for (var c = Start; c <= End; c++)
                {
                    yield return c;
                }
            }
        }

        public CharacterRegion() { }

        public CharacterRegion(char start, char end)
        {
            Start = start;
            End = end;
        }
    }
}
