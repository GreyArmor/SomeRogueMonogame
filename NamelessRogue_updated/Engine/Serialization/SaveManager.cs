using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using FlatSharp;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Serialization.AutogeneratedSerializationClasses;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using NamelessRogue.shell;
using Newtonsoft.Json;
using Polenter.Serialization;

namespace NamelessRogue.Engine.Serialization
{

    public static class ObjectExtensions
    {
        public static T CastTo<T>(this object o) => (T)o;

        public static dynamic CastToReflected(this object o, Type type)
        {
            var methodInfo = typeof(ObjectExtensions).GetMethod(nameof(CastTo), BindingFlags.Static | BindingFlags.Public);
            var genericArguments = new[] { type };
            var genericMethodInfo = methodInfo?.MakeGenericMethod(genericArguments);
            return genericMethodInfo?.Invoke(null, new[] { o });
        }
    }

    public class SaveManager
    {        

        public static void LoadGame(String pathToFolder, NamelessGame game)
        {
            //var saveFile = new NamelessRogueSaveFile();

        }

        public static void SaveGame(String pathToFolder, NamelessGame game)
        {
            var saveFile = new NamelessRogueSaveFile();
            Type iStorageInterfaceType = typeof(IStorage<>);

            foreach (var componentTypeCollection in EntityInfrastructureManager.Components)
            {
                if (componentTypeCollection.Key.GetCustomAttributes(true).Any(a => a.GetType() == typeof(NamelessRogue.Engine.Serialization.SkipClassGeneration)))
                {
                    continue;
                }

                foreach (var componentDictionary in componentTypeCollection.Value)
                {



                    //the type of the storage that can store current type
                    Type storageType = saveFile.ComponentTypeToStorge[componentTypeCollection.Key];
                        
                    var constructor = storageType.GetConstructor(Type.EmptyTypes);
                    if (constructor == null)
                    {
                        throw new Exception($@"{storageType.Name} must contain a parameterless constructor in order for Save manager to serialize it");
                    }



                    dynamic storageObject = constructor.Invoke(null);

                    storageObject.FillFrom(ObjectExtensions.CastToReflected(componentDictionary.Value, componentDictionary.Value.GetType()));

                    saveFile.StoragesDictionary[storageObject.GetType()].Add(storageObject);
                }
            }

            int maxBytesNeeded = FlatBufferSerializer.Default.GetMaxSize(saveFile);
            byte[] buffer = new byte[maxBytesNeeded];
            int bytesWritten = FlatBufferSerializer.Default.Serialize(saveFile, buffer);

            NamelessRogueSaveFile p = FlatBufferSerializer.Default.Parse<NamelessRogueSaveFile>(buffer);

            
        }




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

