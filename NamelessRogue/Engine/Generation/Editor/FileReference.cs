using System.Xml.Serialization;

namespace NamelessRogue.Engine.Generation.Editor
{
    public partial class QuestTemplateData
    {
        [XmlRoot]
        public class FileReference
        {
            public string id = "";
            public string path = "";

            [XmlElement]
            public string Id { get => id; set => id = value; }
            [XmlElement]
            public string Path { get => path; set => path = value; }
        }
    }
}
