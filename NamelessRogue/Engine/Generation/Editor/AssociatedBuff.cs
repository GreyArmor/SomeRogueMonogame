using System.Xml.Serialization;

namespace NamelessRogue.Engine.Generation.Editor
{
    [XmlRoot]
    public class AssociatedBuff
    {

        [XmlElement]
        public string BuffId { get; set; }
        [XmlElement]
        public string Path { get; set; }
    }
}
