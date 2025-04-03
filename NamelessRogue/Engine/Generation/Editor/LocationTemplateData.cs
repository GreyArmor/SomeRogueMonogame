using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static NamelessRogue.Engine.Generation.Editor.QuestTemplateData;

namespace NamelessRogue.Engine.Generation.Editor
{
    public class LocationTemplateData
    {
        [XmlElement]
        public string Id { get; set; }
        [XmlElement]
        public string Name { get; set; }
        [XmlElement]
        public string Description { get; set; }
        [XmlElement]
        public Vector2 Size { get; set; }
        [XmlElement]
        public FileReference TiledFilePath { get; set; }

    }
}
