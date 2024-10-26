using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NamelessRogue.Engine.Generation.Editor
{

    [XmlRoot]
    public class ItemTemplateData
    {
        [XmlElement]
        public string Name { get; set; }
        [XmlElement]
        public string Description { get; set; }

        [XmlElement]
        public string IconPath { get; set; }

        [XmlElement]
        public ItemType ItemType { get; set; }

        [XmlElement]
        public ItemQuality ItemQuality { get; set; }

        [XmlArray]
        public List<Slot> PossibleSlots { get; set; }

        [XmlElement]
        public WeaponTemplateData? WeapomTemplateData { get; set; }
        [XmlElement]
        public ArmorTemplateData? ArmorTemplateData { get; set; }

    }

    [XmlRoot]
    public class WeaponTemplateData
    {
        [XmlElement]
        public int MinimumDamage { get; set; }
        [XmlElement]
        public int MaximumDamage { get; set; }
        [XmlElement]
        public int Range { get; set; }
        [XmlElement]
        public AttackType AttackType { get; set; }
        [XmlElement]
        public DamageType DamageType { get; set; }
        [XmlElement]
        public AmmoType AmmoType { get; set; }
        [XmlElement]
        public int AmmoInClip { get; set; }
    }

    [XmlRoot]
    public class ArmorTemplateData
    {
        [XmlElement]
        public DamageType DamageType { get; set; }

        [XmlElement]
        public int ArmorValue { get; set; }

        [XmlElement]
        public DamageType ResistType { get; set; }

        [XmlElement]
        public int ResistValue{ get; set; }
    }
}
