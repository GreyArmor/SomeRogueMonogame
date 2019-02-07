using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;

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

            var formatter = new DataContractSerializer(typeof(Chunk));
            var settings = new XmlWriterSettings { Indent = true };

            var smmWriter = XmlWriter.Create(pathToFolder+ "\\" + chunkId+".xml", settings);

            formatter.WriteObject(smmWriter, chunk);
            smmWriter.Close();

          
        }

        public static Chunk LoadChunk(String pathToFolder, String chunkId)
        {
            var formatter = new DataContractSerializer(typeof(Chunk));
            var stream = new FileStream(pathToFolder + "\\" + chunkId + ".xml", FileMode.Open, FileAccess.Read);
            var chunk = (Chunk)formatter.ReadObject(stream);
            stream.Close();
            return chunk;
        }
    }
}

