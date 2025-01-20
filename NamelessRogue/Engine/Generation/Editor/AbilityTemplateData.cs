using NamelessRogue.Engine.Components.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NamelessRogue.Engine.Generation.Editor
{
    [XmlRoot]
    public class AbilityTemplateData
    {
        [XmlElement]
        public string Id { get; set; }
        [XmlElement]
        public string Name { get; set; }
        [XmlElement]
        public string Description { get; set; }
        [XmlElement]
        public string IconPath { get;  set; }
        [XmlElement]
        public int ActionPointsCost { get; set; }
        [XmlElement]
        public int EnergyCost { get; set; }
        [XmlElement]
        public TargetMode TargetMode { get; set; }
        [XmlElement]
        public ActivationMode ActivationMode { get; set; }
        [XmlElement]
        public int AreaOfEffect { get; set; } = 0;
        [XmlElement]
        public bool IsActive { get; set; }
        [XmlElement]
        public int Range { get; set; } = 0;
        [XmlElement]
        public int CooldownTurns { get; set; } = 0;
        [XmlArray]
        public List<AssociatedBuff> AssociatedBuffs { get; set; }
        [XmlArray]
        public List<AbilityAction> AbilityActions { get; set; }
    }
}
