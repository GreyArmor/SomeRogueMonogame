using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using NamelessRogue.Engine.Generation.Editor;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System.Drawing;
using NamelessRogue.Engine.Abstraction;

namespace NamelessRogue.Engine.Factories
{
    public static class BuildingLibrary
    {
        public static List<BuildingTemplateData> Data = new List<BuildingTemplateData>();
        public static Dictionary<string, BuildingTemplateData> DataById = new Dictionary<string, BuildingTemplateData>();
        public static void LoadData(NamelessGame game)
        {
            var buffs = Directory.GetFiles(Environment.CurrentDirectory + Constants.GameObjectRelativePath, "*.nrlf", SearchOption.AllDirectories);
            foreach (var itemPath in buffs)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(BuildingTemplateData));
                TextReader reader = new StreamReader(itemPath);

                var data = (BuildingTemplateData)serializer?.Deserialize(reader);
                Data.Add(data);
                DataById.Add(data.Id, data);
            }
        }

        public static void ClearData()
        {
            Data.Clear();
            DataById.Clear();
        }

        public static IEntity CreateBuildingFromData(NamelessGame game, Point worldCoordinate, BuildingTemplateData data)
        {
           return BuildingFactory.CreateBuilding(worldCoordinate.X, worldCoordinate.Y, data, game);
        }

    }

}

