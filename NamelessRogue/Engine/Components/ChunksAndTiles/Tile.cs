using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using SharpDX;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Components.ChunksAndTiles
{
    
    public class Tile {
        
        public Biomes Biome { get; set; }

        public Tile(){}
	
        public Tile(TerrainTypes terrain, Biomes biome, Point coordinate, double elevation)
        {
            this.coordinate = coordinate;
            Terrain = terrain;
            Biome = biome;
            this.Elevation = elevation;
        }
        public double Elevation { get; set; }
        //TODO: should not be here
        public float ElevationVisual { get; set; }
        public TerrainTypes Terrain { get; set; }
        
        private Point coordinate;
        
        private List<Entity> entitiesOnTile = new List<Entity>();

        public bool AnyEntities()
        {
            return entitiesOnTile.Any();
        }


        public List<Entity> GetEntities()
        {
            return entitiesOnTile.ToList();
        }

        public void SetEntities(List<Entity> entities)
        {
            entitiesOnTile = entities.ToList();
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

		public bool IsPassableIgnoringCharacters()
		{
			foreach (var entity in entitiesOnTile)
			{
				var occupiesTile = entity.GetComponentOfType<OccupiesTile>();
				var character = entity.GetComponentOfType<Character>();
				if (occupiesTile != null && character== null)
				{
					return false;
				}
			}
			return true;
		}

		public bool GetBlocksVision(NamelessGame game)
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
    }
}
