using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using NamelessRogue.Engine.Generation.Editor;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Factories
{
    public static class DialogLibrary
    {
        public static List<DialogData> Data = new List<DialogData>();
        public static Dictionary<string, DialogData> DataById = new Dictionary<string, DialogData>();
        public static void LoadData(NamelessGame game)
        {
            var items = Directory.GetFiles(Environment.CurrentDirectory + Constants.GameObjectRelativePath, "*.nrdf", SearchOption.AllDirectories);
            foreach (var dataPath in items)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DialogData));
                TextReader reader = new StreamReader(dataPath);

                var dialogData = (DialogData)serializer?.Deserialize(reader);

                Data.Add(dialogData);
                DataById.Add(dialogData.Id, dialogData);
            }
        }

        public static void ClearData()
        {
            Data.Clear();
            DataById.Clear();
        }

    }
}

