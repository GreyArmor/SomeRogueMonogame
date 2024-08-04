using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using FlatSharp;

using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Serialization.AutogeneratedSerializationClasses;
using NamelessRogue.Engine.Serialization.CustomSerializationClasses;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using NamelessRogue.shell;
using Newtonsoft.Json;

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
        public static void Init()
        {
            FlatBufferSerializer.Default.Compile<NamelessRogueSaveFile>();
            FlatBufferSerializer.Default.Compile<TimelineStorage>();
        }
        public static void SaveGame(String pathToFolder, NamelessGame game)
        {
            return;
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
            foreach (var entity in EntityInfrastructureManager.Entities.Values)
            {
                var storage = new EntityStorage();
                storage.FillFrom(entity);
                saveFile.EntityStorageTable.Add(storage);
            }

            {
                int maxBytesNeeded = FlatBufferSerializer.Default.GetMaxSize(saveFile);
                byte[] buffer = new byte[maxBytesNeeded];
                int bytesWritten = FlatBufferSerializer.Default.Serialize(saveFile, buffer);

                var stream = File.OpenWrite("objectdata.nrs");
                stream.Write(buffer, 0, bytesWritten);
                stream.Close();
            }

            {
                TimelineStorage timelinesStorage = new TimelineStorage();
                var timeline = game.TimelineEntity.GetComponentOfType<TimeLine>();
                timelinesStorage.FillFrom(timeline);
                {

                    FlatBufferSerializer serializer = new FlatBufferSerializer(FlatBufferDeserializationOption.Greedy);
                    int maxBytesNeeded = serializer.GetMaxSize(timelinesStorage);

                    int byteNeeded = serializer.GetMaxSize(timelinesStorage.CurrentTimelineLayer.WorldTiles[50 * 10]);

                    byte[] buffer = new byte[maxBytesNeeded];

                    int bytesWritten = serializer.Serialize(timelinesStorage, buffer);

                    var stream = File.OpenWrite("chunksmemory.nrs");                
                    stream.Write(buffer,0, bytesWritten);
                    stream.Close();
                }
            }
        }

        public static void LoadGame(String pathToFolder, NamelessGame game)
        {
            {
                Type iStorageInterfaceType = typeof(IStorage<>);
                var buffer = File.ReadAllBytes("objectdata.nrs");
                var saveFile = FlatBufferSerializer.Default.Parse<NamelessRogueSaveFile>(buffer);

                saveFile.ComponentTypeToStorge.Clear();
                saveFile.StoragesDictionary.Clear();
                saveFile.FillInfrastructureCollections();

                List<Entity> entities = new List<Entity>();
                foreach (var entityStorage in saveFile.EntityStorageTable)
                {
                    var entity = new Entity(new Guid(entityStorage.Id));
                    entities.Add(entity);
                }

                foreach (var typePair in saveFile.ComponentTypeToStorge)
                {
                    if (typePair.Key == typeof(Entity) || typePair.Key == typeof(IEntity))
                    {
                        continue;
                    }
                    var enumer = saveFile.StoragesDictionary[typePair.Value].GetEnumerator();

                    while (enumer.MoveNext())
                    {
                        var storage = enumer.Current;

                        Component component = (Component)typePair.Key.GetConstructor(Type.EmptyTypes).Invoke(null);

                        storage.FillTo(component.CastToReflected(typePair.Key));

                        EntityInfrastructureManager.AddComponent(component.ParentEntityId, component);
                    }
   
                }
            }      
            
            {
                var buffer = File.ReadAllBytes("chunksmemory.nrs");
                var saveFile = FlatBufferSerializer.Default.Parse<TimelineStorage>(buffer);
                TimeLine timeLine = new TimeLine();
                saveFile.FillTo(timeLine);

                timeLine.CurrentTimelineLayer.Chunks.WorldBoard = timeLine.CurrentTimelineLayer;
                //TO DO: serialize properly
                timeLine.CurrentTimelineLayer.Chunks.WorldSettings = game.WorldSettings;

                foreach (var chunk in timeLine.CurrentTimelineLayer.Chunks.Chunks)
                {
                    chunk.Value.ChunkContainer = timeLine.CurrentTimelineLayer.Chunks;
                }

                var entity = new Entity(timeLine.ParentEntityId);
                entity.AddComponent(timeLine);

                game.CameraEntity = game.GetEntityByComponentClass<ConsoleCamera>();
                game.Commander = game.GetEntityByComponentClass<Commander>().GetComponentOfType<Commander>();
                game.PlayerEntity = game.GetEntityByComponentClass<Player>();
                game.TimelineEntity = entity;
                game.CursorEntity = game.GetEntityByComponentClass<Cursor>();
                game.FollowedByCameraEntity = game.GetEntityByComponentClass<FollowedByCamera>();
            }
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


        public static void SaveTimelineLayer(String pathToFolder, WorldBoard layer, String id)
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

        public static WorldBoard LoadTimelineLayer(String pathToFolder, String id)
        {

            using (StreamReader reader = new StreamReader(pathToFolder + "\\" + id + ".json"))
            using (JsonTextReader jsonReader = new JsonTextReader(reader))
            {
                JsonSerializer ser = new JsonSerializer();
                return ser.Deserialize<WorldBoard>(jsonReader);
            }
        }
    }
}

