using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Components.ChunksAndTiles
{
    public class Tile {
        public Biomes Biome { get; }

        public Tile(){}
	
        public Tile(TerrainTypes terrainType, Biomes biome, Point coordinate){
            Biome = biome;
            this.terrainType = terrainType;
            this.coordinate = coordinate;
        }
        private TerrainTypes terrainType;
        private Point coordinate;
        private List<IEntity> entitiesOnTile = new List<IEntity>();

        public TerrainTypes getTerrainType() {
            return terrainType;
        }

        public void SetTerrainType(TerrainTypes terrainType) {
            this.terrainType = terrainType;
        }



        public List<IEntity> getEntitiesOnTile() {
            return entitiesOnTile;
        }


        public bool GetPassable(NamelessGame namelessGame) {
            foreach (var entity in entitiesOnTile)
            {
                var occupiesTile = entity.GetComponentOfType<OccupiesTile>();
                if (occupiesTile != null)
                {
                    return false;
                }
            }
            return true;
        }

        public bool GetBlocksVision(NamelessGame namelessGame)
        {
            foreach (var entity in entitiesOnTile)
            {
                var blocksVision = entity.GetComponentOfType<BlocksVision>();
                if (blocksVision != null)
                {
                    return false;
                }
            }
            return true;
        }


        public Point GetCoordinate() {
            return coordinate;
        }
    }
}
