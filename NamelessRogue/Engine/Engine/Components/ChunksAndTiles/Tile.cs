using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Components.ChunksAndTiles
{
    
    public class Tile {
        
        public Biome Biome { get; set; }

        public Tile(){}
	
        public Tile(Terrain terrain, Biome biome, Point coordinate, double elevation)
        {
            this.coordinate = coordinate;
            Terrain = terrain;
            Biome = biome;
            this.Elevation = elevation;
        }
        public double Elevation { get; set; }
        public Terrain Terrain { get; set; }
        
        private Point coordinate;
        
        private List<Entity> entitiesOnTile = new List<Entity>();

        public List<Entity> GetEntities()
        {
            return entitiesOnTile.ToList();
        }

        public void AddEntity(Entity entity)
        {
            entitiesOnTile.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            entitiesOnTile.Remove(entity);
        }

        public bool IsPassable() {
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
