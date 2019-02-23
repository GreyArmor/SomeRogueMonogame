using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Generation.World;
using Newtonsoft.Json;
using Polenter.Serialization;

namespace NamelessRogue.Engine.Engine.Serialization
{
    public class SaveManager
    {
        //        public static void LoadGame(String pathToFolder, NamelessGame game)
        //        {

        //        }

        //        public static void SaveGame(String pathToFolder, NamelessGame game)
        //        {

        //        }

        public static void SaveChunk(String pathToFolder, Chunk chunk, String chunkId) //, NamelessGame game)
        {

            if (!Directory.Exists(pathToFolder))
            {
                Directory.CreateDirectory(pathToFolder);
            }

            string output = JsonConvert.SerializeObject(chunk);

            File.WriteAllText(pathToFolder + "\\" + chunkId + ".json", output);

        }

        public static Chunk LoadChunk(String pathToFolder, String chunkId)
        {
            var text = File.ReadAllText(pathToFolder + "\\" + chunkId + ".json");
            Chunk chunk = JsonConvert.DeserializeObject<Chunk>(text);
            return chunk;
        }


        public static void SaveTimelineLayer(String pathToFolder, TimelineLayer layer, String id)
        {
            if (!Directory.Exists(pathToFolder))
            {
                Directory.CreateDirectory(pathToFolder);
            }

            using (StreamWriter writer = new StreamWriter(pathToFolder + "\\" + id + ".json"))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                JsonSerializer ser = new JsonSerializer();
                ser.NullValueHandling = NullValueHandling.Ignore;
                ser.Serialize(jsonWriter, layer);
                jsonWriter.Flush();
            }




        }

        public static TimelineLayer LoadTimelineLayer(String pathToFolder, String id)
        {

            using (StreamReader reader = new StreamReader(pathToFolder + "\\" + id + ".json"))
            using (JsonTextReader jsonReader = new JsonTextReader(reader))
            {
                JsonSerializer ser = new JsonSerializer();
                return ser.Deserialize<TimelineLayer>(jsonReader);
            }
        }
    }
}

