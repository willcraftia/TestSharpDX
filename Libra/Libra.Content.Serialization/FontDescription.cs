#region Using

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

#endregion

namespace Libra.Content.Serialization
{
    [DataContract]
    public sealed class FontDescription
    {
        string fontName;

        float size;

        float spacing;

        [DataMember]
        public char? DefaultCharacter { get; set; }

        [DataMember]
        public string FontName
        {
            get { return fontName; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                fontName = value;
            }
        }

        [DataMember]
        public float Size
        {
            get { return size; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("value");
                
                size = value;
            }
        }

        [DataMember]
        public float Spacing
        {
            get { return spacing; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("value");

                spacing = value;
            }
        }

        // TODO
        //
        // 現在未使用。

        [DataMember]
        public bool UseKerning { get; set; }

        [DataMember]
        public FontDescriptionStyle Style { get; set; }

        [DataMember]
        public List<CharacterRegion> CharacterRegions { get; private set; }

        //[IgnoreDataMember]
        public ICollection<char> Characters { get; private set; }

        public FontDescription()
        {
            CharacterRegions = new List<CharacterRegion>();
        }

        public void InitializeCharacters()
        {
            Characters = new List<char>();

            if (CharacterRegions.Any())
            {
                var characters = CharacterRegions.SelectMany(region => region.Characters).Distinct();
                foreach (var c in characters)
                {
                    Characters.Add(c);
                }
            }

            if (DefaultCharacter.HasValue)
            {
                Characters.Add(DefaultCharacter.Value);
            }
        }
    }
}
