using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NamelessRogue.Engine.Generation.Editor
{
    public class BuffTemplateData
    {
        [XmlElement]
        public string Id { get; set; }
        [XmlElement]
        public string Name { get; set; }
        [XmlElement]
        public string Description { get; set; }

        [XmlElement]
        public string IconPath { get; set; }

        [XmlElement]
        public int Duration { get; set; }

        [XmlElement]
        public bool IsAppliedImmediately { get; set; }

        [XmlElement]
        public bool IsDamageOverTime { get; set; }

        [XmlElement]
        public int HealthModificator { get; set; } = 0;

        [XmlElement]
        public int EnergyModificator { get; set; } = 0;

        [XmlElement]
        public int ArmorModificator { get; set; } = 0;

        [XmlElement]
        public int ResistanceModificator { get; set; } = 0;

        [XmlElement]
        public int DamageModificator { get; set; } = 0;
    }
}
