using NamelessRogue.Engine.Components.Interaction;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NamelessRogue.Engine.Generation.Editor
{
    public partial class QuestTemplateData
    {
        [XmlRoot]
        public class QuestObjectiveData
        {

            [XmlArray]
            public List<FileReference> Npcs { get; set; } = new List<FileReference>();

            [XmlArray]
            public List<FileReference> Items { get; set; } = new List<FileReference>();

            [XmlArray]
            public List<FileReference> Locations { get; set; } = new List<FileReference>();

        }
    }
}
