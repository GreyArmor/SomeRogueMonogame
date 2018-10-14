using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.Infrastructure;

namespace NamelessRogue.Engine.Engine.Components.ChunksAndTiles
{
    public class Tile {

        public Tile(){}
	
        public Tile(TerrainTypes terrainType, Point coordinate){
            this.terrainType = terrainType;
            this.coordinate = coordinate;
            this.isPassable = true;
        }
        private TerrainTypes terrainType;
        private Point coordinate;
        private bool isPassable;
        private   List<Entity> entitiesOnTile = new List<Entity>();

        public TerrainTypes getTerrainType() {
            return terrainType;
        }

        public void SetTerrainType(TerrainTypes terrainType) {
            this.terrainType = terrainType;
        }



        public List<Entity> getEntitiesOnTile() {
            return entitiesOnTile;
        }

        public void setPassable(bool passable) {
            isPassable = passable;
        }

        public bool getPassable() {
            return isPassable;
        }


        public Point getCoordinate() {
            return coordinate;
        }
    }
}
