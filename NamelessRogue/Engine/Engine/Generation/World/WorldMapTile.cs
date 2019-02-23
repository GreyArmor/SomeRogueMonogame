

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.Generation.World.BoardPieces;
using NamelessRogue.Engine.Engine.Infrastructure;

namespace NamelessRogue.Engine.Engine.Generation.World
{
   
    public class WorldTile {
        public Terrain Terrain { get; set; }
        public Biome Biome { get; set; }
        public MapResource Resource { get; set; }
        public MapBuilding Building { get; set; }
        public MapArtifact Artifact { get; set; }
        public MetaphysicalForce Affinity { get; set; } = new MetaphysicalForce("",new Color());
        public Civilization Owner { get; set; }
        public Region Continent { get; set; }
        public Region LandmarkRegion { get; set; }
        public Settlement Settlement { get; set; }


        private Point point;

        public WorldTile(Point point)
        {
            this.WorldBoardPosiiton = point;
        }

        public Point WorldBoardPosiiton
        {
            get { return point; }
            set { point = value; }
        }
    }
}
