

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Generation.World.BoardPieces;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Generation.World
{
   
    public class WorldTile {
        public TerrainTypes Terrain { get; set; }
        public Biomes Biome { get; set; }
        public MapBuilding[] Buildings { get; set; } = Array.Empty<MapBuilding>();
        public float Elevation { get; set; }
        public bool RiverMapValue { get; set; }
        public bool RiverMapBorderValue { get; set; }
        public bool RoadMapValue { get; set; }
        private Point point;

        public WorldTile(Point point)
        {
            this.WorldBoardPosiiton = point;
        }

		public WorldTile()
		{
		}

		public Point WorldBoardPosiiton
        {
            get { return point; }
            set { point = value; }
        }
    }
}
