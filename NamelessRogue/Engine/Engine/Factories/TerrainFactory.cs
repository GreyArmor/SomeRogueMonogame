using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Infrastructure;

namespace NamelessRogue.Engine.Engine.Factories
{
    public class TerrainFactory {
        public static Entity CreateWorld()
        {
            Entity world = new Entity();
            ChunkData chunkData = new ChunkData();
            world.AddComponent(chunkData);		
            return world;
        }
	
    }
}
