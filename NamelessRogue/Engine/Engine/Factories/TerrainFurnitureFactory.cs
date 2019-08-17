using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeonBit.UI.Entities;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Components.Environment;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.Engine.Engine.Components.UI;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.Engine.Engine.Utility;
using NamelessRogue.shell;
using Entity = NamelessRogue.Engine.Engine.Infrastructure.Entity;

namespace NamelessRogue.Engine.Engine.Factories
{
    public static class TerrainFurnitureFactory
    {
        //not sure how clear this is
        public static Entity TreeEntity = new Entity();
        public static Entity SmallTreeEntity = new Entity();
        public static Entity RockEntity = new Entity();
        public static Entity StarfishEntity = new Entity();
        public static Entity ShellEntity = new Entity();
        public static Entity TreeStumpEntity = new Entity();
        public static Entity WallEntity = new Entity();
        public static Entity WindowEntity = new Entity();
        public static Entity BedEntity = new Entity();
        public static Entity BarrelEntity = new Entity();
        public static Entity FlowerEntity = new Entity();
        public static Entity TableEntity = new Entity();
        public static Entity ChairEntity = new Entity();


        public static List<Entity> CreateInstancedFurnitureEntities(NamelessGame game)
        {
            var result = new List<Entity>();
            result.Add(TreeEntity);
            result.Add(SmallTreeEntity);
            result.Add(RockEntity);
            result.Add(StarfishEntity);
            result.Add(ShellEntity);
            result.Add(TreeStumpEntity);
            result.Add(WallEntity);
            result.Add(WindowEntity);
            result.Add(BedEntity);
            result.Add(BarrelEntity);
            result.Add(FlowerEntity);

            StarfishEntity.AddComponent(new Description("A starfish", ""));
            StarfishEntity.AddComponent(new Drawable('★', new Color(1f, 0, 0)));

            ShellEntity.AddComponent(new Description("A shell", ""));
            ShellEntity.AddComponent(new Drawable('Q', new Color(0.8f, 0.8f, 0.5f)));

            RockEntity.AddComponent(new Description("A rock", ""));
            RockEntity.AddComponent(new Drawable('o', new Color(0.5f, 0.5f, 0.5f)));
            RockEntity.AddComponent(new Item());

            TreeEntity.AddComponent(new Description("A tree", ""));
            TreeEntity.AddComponent(new BlocksVision());
            TreeEntity.AddComponent(new OccupiesTile());
            TreeEntity.AddComponent(new Drawable('T', new Color(0f, 0.5f, 0f)));

            SmallTreeEntity.AddComponent(new Description("A small tree", ""));
            SmallTreeEntity.AddComponent(new Drawable('t', new Color(0f, 0.5f, 0f)));

            TreeStumpEntity.AddComponent(new Description("A tree stump", ""));
            TreeStumpEntity.AddComponent(new Drawable('u', new Color(0.5f, 0.5f, 0f)));
            
            WallEntity.AddComponent(new Drawable('#', new Engine.Utility.Color(0.9, 0.9, 0.9)));
            WallEntity.AddComponent(new Description("Wall", ""));
            WallEntity.AddComponent(new OccupiesTile());
            WallEntity.AddComponent(new BlocksVision());

            WindowEntity.AddComponent(new Drawable('O', new Engine.Utility.Color(0, 0.9, 0.9)));
            WindowEntity.AddComponent(new Description("Window", ""));

            BedEntity.AddComponent(new Drawable('B', new Engine.Utility.Color(139, 69, 19)));
            BedEntity.AddComponent(new Description("Bed", ""));

            BarrelEntity.AddComponent(new Drawable('O', new Engine.Utility.Color(139, 69, 19)));
            BarrelEntity.AddComponent(new Description("Barrel", ""));

            WindowEntity.AddComponent(new OccupiesTile());
            foreach (var entity in result)
            {
                entity.AddComponent(new Instanced());
                entity.AddComponent(new Furniture());
            }
           
            return result;
        }

        public static Entity GetExteriorEntities(NamelessGame game, Tile terrainTile)
        {
            var random = game.WorldSettings.GlobalRandom;
            Entity result = null;
            switch (terrainTile.Biome.Type)
            {
                case Biomes.Beach:
                {
                    result = new Entity();
                    var randomValue = random.NextDouble();
                    if (randomValue > 0.997)
                    {
                        result = StarfishEntity;
                    }
                    else if (randomValue > 0.985)
                    {
                        result = ShellEntity;
                    }
                    else if (randomValue > 0.98)
                    {
                        result = RockEntity;
                    }

                    break;
                }
                case Biomes.Forest:
                {
                    var randomValue = random.NextDouble();
                    if (randomValue > 0.95)
                    {
                        result = TreeEntity;
                    }else if (randomValue > 0.90)
                    {
                        result = SmallTreeEntity;
                    }
                    else if (randomValue > 0.89)
                    {
                        result = TreeStumpEntity;
                    }

                        break;
                }
                case Biomes.Desert:
                {
                    var randomValue = random.NextDouble();
                    if (randomValue > 0.98)
                    {
                        result = RockEntity;
                    }

                }
                    break;
                default:
                    break;;
            }

            return result;
        }
    }
}
