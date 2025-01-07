using NamelessRogue.Engine.Components.ItemComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NamelessRogue.Engine.Generation.Editor
{

    [XmlRoot]
    public class CharacterTemplateData
    {
        [XmlElement]
        public string Name { get; set; }
        [XmlElement]
        public string Description { get; set; }

        [XmlElement]
        public string SpritePath { get; set; }

        [XmlElement]
        public bool CastsShadow { get; set; }

        [XmlElement]
        public bool Immobile { get;  set; }

        [XmlElement]
        public int Health { get; set; }

        [XmlElement]
        public int Energy { get; set; }

        [XmlElement]
        public int MovementSpeed { get;  set; }

        [XmlElement]
        public string FactionId { get; set; }

        [XmlArray]
        public List<DroppedItem> DroppedItems { get; set; }

        [XmlElement]
        public WeaponTemplateData WeaponTemplateData { get; set; }
        [XmlElement]
        public ArmorTemplateData ArmorTemplateData { get; set; }


    }

    [XmlRoot]
    public class DroppedItem
    {
        [XmlElement]
        public string Path { get; set; }
        /// <summary>
        /// between 0 and 100;
        /// </summary>
        [XmlElement]
        public int Probability { get; set; } = 0;
    }

}
