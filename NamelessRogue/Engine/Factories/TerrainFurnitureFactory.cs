using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Entity = NamelessRogue.Engine.Infrastructure.Entity;

namespace NamelessRogue.Engine.Factories
{
    public static class TerrainFurnitureFactory
    {
        //not sure how clear this is
        private static Entity treeEntity = new Entity();
        private static Entity smallTreeEntity = new Entity();
        private static Entity rockEntity = new Entity();
        private static Entity starfishEntity = new Entity();
        private static Entity shellEntity = new Entity();
        private static Entity treeStumpEntity = new Entity();

        private static Entity wallEntity = new Entity();
        private static Entity windowEntity = new Entity();
        private static Entity bedEntity = new Entity();
        private static Entity barrelEntity = new Entity();
        private static Entity flowerEntity = new Entity();
        private static Entity tableEntity = new Entity();
        private static Entity chairEntity = new Entity();
        private static Entity boxEntity = new Entity();
        private static Entity garbageEntity = new Entity();

        public static Entity WallEntity { get => (Entity)wallEntity.CloneEntity(); }
        public static Entity WindowEntity { get => (Entity)windowEntity.CloneEntity(); }
        public static Entity BedEntity { get => (Entity)bedEntity.CloneEntity(); }
        public static Entity BarrelEntity { get => (Entity)barrelEntity.CloneEntity(); }
        public static Entity FlowerEntity { get => (Entity)flowerEntity.CloneEntity(); }
        public static Entity TableEntity { get => (Entity)tableEntity.CloneEntity(); }
        public static Entity ChairEntity { get => (Entity)chairEntity.CloneEntity(); }
        public static Entity TreeStumpEntity { get => (Entity)treeStumpEntity.CloneEntity(); }
        public static Entity ShellEntity { get => (Entity)shellEntity.CloneEntity(); }
        public static Entity StarfishEntity { get => (Entity)starfishEntity.CloneEntity(); }
        public static Entity RockEntity { get => (Entity)rockEntity.CloneEntity(); }
        public static Entity SmallTreeEntity { get => (Entity)smallTreeEntity.CloneEntity(); }
        public static Entity TreeEntity { get => (Entity)treeEntity.CloneEntity(); }

        public static Entity BoxEntity { get => (Entity)boxEntity.CloneEntity(); }

        public static Entity GarbageEntity { get => (Entity)boxEntity.CloneEntity(); }

        public static void CreateFurnitureEntities(NamelessGame game)
        {
            var result = new List<Entity>();
            result.Add(treeEntity);
            result.Add(smallTreeEntity);
            result.Add(rockEntity);
            result.Add(starfishEntity);
            result.Add(shellEntity);
            result.Add(treeStumpEntity);
            result.Add(wallEntity);
            result.Add(windowEntity);
            result.Add(bedEntity);
            result.Add(barrelEntity);
            result.Add(flowerEntity);
            result.Add(boxEntity);
            result.Add(garbageEntity);

            rockEntity.AddComponent(new Description("A rock", ""));
            rockEntity.AddComponent(new Drawable("Rock", new Color(0.5f, 0.5f, 0.5f)));
            rockEntity.AddComponent(new Item(ItemType.Misc, 2, ItemQuality.Normal, 1, 1, ""));

            treeEntity.AddComponent(new Description("A tree", ""));
            treeEntity.AddComponent(new BlocksVision());
            treeEntity.AddComponent(new OccupiesTile());
            treeEntity.AddComponent(new Drawable("Tree", new Color(0f, 0.5f, 0f)));

            smallTreeEntity.AddComponent(new Description("A small tree", ""));
            smallTreeEntity.AddComponent(new Drawable("SnallTree", new Color(0f, 0.5f, 0f)));

            wallEntity.AddComponent(new Drawable("Wall", new Engine.Utility.Color(0.5f)));
            wallEntity.AddComponent(new Description("Wall", ""));
            wallEntity.AddComponent(new OccupiesTile());
            wallEntity.AddComponent(new BlocksVision());

            windowEntity.AddComponent(new Drawable("Window", new Engine.Utility.Color(0, 0.9, 0.9)));
            windowEntity.AddComponent(new Description("Window", ""));

            bedEntity.AddComponent(new Drawable("Bed", new Engine.Utility.Color(139, 69, 19)));
            bedEntity.AddComponent(new Description("Bed", ""));

            barrelEntity.AddComponent(new Drawable("Barrel", new Engine.Utility.Color(139, 69, 19)));
            barrelEntity.AddComponent(new Description("Barrel", ""));

            boxEntity.AddComponent(new Drawable("Box", new Engine.Utility.Color(139, 69, 19)));
            boxEntity.AddComponent(new Description("Box", ""));

            garbageEntity.AddComponent(new Description("Garbage", ""));

            windowEntity.AddComponent(new OccupiesTile());
            foreach (var entity in result)
            {
                entity.AddComponent(new Furniture());
            }

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
                case TerrainTypes.Sidewalk:
                    {
                        var randomValue = random.Next(0, 100) / 100d;
                        if (randomValue > 0.96)
                        {
                            var garbage = new Entity();

                            var randomGarbageNumber = random.Next(1, 22);

                            //to get a random sprite
                            garbage.AddComponent(new Description("Garbage", ""));
                            garbage.AddComponent(new Drawable(@$"garbage{randomGarbageNumber}", new Engine.Utility.Color(255, 255, 255)));
                            garbage.AddComponent(new Furniture());
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
