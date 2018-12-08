using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeonBit.UI.Entities;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
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
    public static class TerrainItemsFactory
    {
        public static IEntity CreateExteriorEntity(NamelessGame game, Tile terrainTile)
        {
            var random = game.WorldSettings.GlobalRandom;
            IEntity result = null;
            switch (terrainTile.Biome)
            {
                case Biomes.Beach:
                    result = new Entity();
                    var randomValue = random.NextDouble();
                    if (randomValue > 0.99)
                    {
                        result.AddComponent(new Description("A starfish", ""));
                        result.AddComponent(new Drawable('★', new Color(1f, 0, 0)));
                    }
                    else if(randomValue>0.98)
                    {
                        result.AddComponent(new Description("A shell", ""));
                        result.AddComponent(new Drawable('Q', new Color(0.5f, 0.5f, 0.5f)));
                    }
                    else if (randomValue > 0.97)
                    {
                        result.AddComponent(new Description("A rock", ""));
                        result.AddComponent(new Drawable('o', new Color(0.5f, 0.5f, 0.5f)));
                    }
                    result.AddComponent(new Movable());
                    result.AddComponent(new Position(terrainTile.GetCoordinate().X, terrainTile.GetCoordinate().Y));
                    break;
                default:
                    break;;
            }

            return result;
        }
    }
}
