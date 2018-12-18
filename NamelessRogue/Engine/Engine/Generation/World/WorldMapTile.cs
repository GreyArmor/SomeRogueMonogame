

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.Generation.World.BoardPieces;
using NamelessRogue.Engine.Engine.Infrastructure;

namespace NamelessRogue.Engine.Engine.Generation.World
{
    public class WorldTile {
        public TerrainTypes Terrain;
        public Biomes Biome;
        public MapResource Resource;
        public MapBuilding Building;
        public MapArtifact Artifact;
        public MetaphysicalForce Affinity;
        public Civilization Owner;
        public Region Continent;
        public Region LandmarkRegion;
        private Point point;

        public WorldTile(Point point)
        {
            this.point = point;
        }

        public Point WorldBoardPosiiton
        {
            get { return point; }
            set { point = value; }
        }
    }
}
