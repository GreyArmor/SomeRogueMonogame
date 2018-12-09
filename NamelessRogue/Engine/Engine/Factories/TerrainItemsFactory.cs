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
        //not sure how clear it is
        public static Entity TreeEntity;
        public static Entity RockEntity;
        public static Entity StarfishEntity;
        public static Entity ShellEntity;
        public static List<Entity> CreateInstancedFurnitureEntities(NamelessGame game)
        {
            var result = new List<Entity>();
            TreeEntity = new Entity();
            RockEntity = new Entity();
            StarfishEntity = new Entity();
            ShellEntity = new Entity();

            result.Add(TreeEntity);
            result.Add(RockEntity);
            result.Add(StarfishEntity);
            result.Add(ShellEntity);

            StarfishEntity.AddComponent(new Description("A starfish", ""));
            StarfishEntity.AddComponent(new Drawable('★', new Color(1f, 0, 0)));

            ShellEntity.AddComponent(new Description("A shell", ""));
            ShellEntity.AddComponent(new Drawable('Q', new Color(0.8f, 0.8f, 0.5f)));

            RockEntity.AddComponent(new Description("A rock", ""));
            RockEntity.AddComponent(new Drawable('o', new Color(0.5f, 0.5f, 0.5f)));

            TreeEntity.AddComponent(new Description("A tree", ""));
            TreeEntity.AddComponent(new BlocksVision());
            TreeEntity.AddComponent(new OccupiesTile());
            TreeEntity.AddComponent(new Drawable('T', new Color(0f, 0.8f, 0f)));


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
            switch (terrainTile.Biome)
            {
                case Biomes.Beach:
                {
                    result = new Entity();
                    var randomValue = random.NextDouble();
                    if (randomValue > 0.999)
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
                    }
                    break;
                }
                default:
                    break;;
            }

            return result;
        }
    }
}
