using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.ECS;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using static Assimp.Metadata;
using Entity = NamelessRogue.Engine.Infrastructure.Entity;

namespace NamelessRogue.Engine.Factories
{
    public static class TerrainFurnitureFactory
    {
        private static Dictionary<string, Entity> FurnitureDisctionary = new Dictionary<string,Entity>();

        public static Entity GetFurniture(string id)
        {
            return (Entity)(FurnitureDisctionary.TryGetValue(id, out var entity) ? entity.CloneEntity() : null);
        }
        public static void CreateFurnitureEntities(NamelessGame game)
        {

            Entity _addFurniture(string id, string descriptionName, bool occupiesTile, bool blocksVision)
            {
                Entity entity = new Entity();
                entity.AddComponent(new Drawable(id, new Engine.Utility.Color(1f)));
                entity.AddComponent(new Description(descriptionName, ""));
                if (occupiesTile)
                {
                    entity.AddComponent(new OccupiesTile());
                }
                if (blocksVision)
                {
                    entity.AddComponent(new BlocksVision());
                }
                entity.AddComponent(new Furniture());
                FurnitureDisctionary.Add(id, entity);
                return entity;
            }


            _addFurniture("wall", "Wall", true, true);
            _addFurniture("window", "Window", true, false);
            _addFurniture("bed", "Bed", false, false);
            _addFurniture("toilet", "Toilet", false, false);
            _addFurniture("shower", "Shower", false, false);
            _addFurniture("barrel", "Barrel", false, false);
            _addFurniture("box", "Box", false, false);
            _addFurniture("table", "Table", true, false);
            _addFurniture("garbage", "Garbage", false, false);


            var stairsDown = _addFurniture("stairs_down", "Stairs", false, false);
            stairsDown.AddComponent(new StairsComponent());
            var stairsUp = _addFurniture("stairs_up", "Stairs", false, false);
            stairsUp.AddComponent(new StairsComponent());
        }

        public static Entity GetExteriorEntities(NamelessGame game, Tile terrainTile)
        {
            var random = game.WorldSettings.GlobalRandom;
            Entity result = null;
            //switch (terrainTile.Biome)
            //{
            //    case Biomes.Beach:
            //        {
            //            var randomValue = random.Next(0, 10000);
            //            if (randomValue > 9997)
            //            {
            //                result = starfishEntity;
            //            }
            //            else if (randomValue > 9996)
            //            {
            //                result = shellEntity;
            //            }
            //            else if (randomValue > 9995)
            //            {
            //                result = rockEntity;
            //            }

            //            break;
            //        }
            //    case Biomes.Forest:
            //        {
            //            var randomValue = random.Next(0, 100) / 100d;
            //            if (randomValue > 0.80)
            //            {
            //                result = treeEntity;
            //            }
            //            else if (randomValue > 0.75)
            //            {
            //                result = smallTreeEntity;
            //            }
            //            else if (randomValue > 0.74)
            //            {
            //                result = treeStumpEntity;
            //            }

            //            break;
            //        }
            //    case Biomes.Desert:
            //        {
            //            var randomValue = random.Next(0, 100) / 100d;
            //            if (randomValue > 0.98)
            //            {
            //                result = rockEntity;
            //            }

            //        }
            //        break;
            //    default:
            //        break; ;
            //}


            switch (terrainTile.Terrain)
            {
                case TerrainTypes.SidewalkPoor:
                case TerrainTypes.AsphaultPoor:
                case TerrainTypes.PaintedAsphault:
                    {
                        var randomValue = random.Next(0, 100) / 100d;
                        if (randomValue > 0.97)
                        {
                            var garbage = new Entity();

                            var randomGarbageNumber = random.Next(1, 21);

                            //to get a random sprite
                            garbage.AddComponent(new Description("Garbage", ""));
                            garbage.AddComponent(new Drawable(@$"garbage{randomGarbageNumber}", new Engine.Utility.Color(255, 255, 255)));
                            garbage.AddComponent(new SpritedObject());
                            result = garbage;
                        }
                    }
                    break;
                default:
                    break;
            }
            if (result == null)
            {
                return null;
            }

            return (Entity)result.CloneEntity();
        }
    }
}
