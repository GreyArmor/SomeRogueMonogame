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
    public class BuildingTemplateData
    {
        public string id = "";
        public string name = "";
        public string description = "";
        public Vector2 size;

        [XmlElement]
        public string Id { get => id; set => id = value; }
        [XmlElement]
        public string Name { get => name; set => name = value; }
        [XmlElement]
        public string Description { get => description; set => description = value; }
        [XmlElement]
        public Vector2 Size { get => size; set => size = value; }
        [XmlArray]
        public List<BuildingFloor> TiledFilePaths { get; set; } = new List<BuildingFloor>();
    }

    public class BuildingFloor
    {
        public int floor = 0;

        [XmlElement]
        public int Floor { get => floor; set => floor = value; }

        [XmlElement]
        public FileReference TiledFilePath { get; set; }
    }

}
