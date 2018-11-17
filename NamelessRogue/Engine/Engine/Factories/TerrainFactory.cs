using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Generation;
using NamelessRogue.Engine.Engine.Infrastructure;

namespace NamelessRogue.Engine.Engine.Factories
{
    public class TerrainFactory {
        public static Entity CreateWorld(WorldSettings settings)
        {
            Entity world = new Entity();
            ChunkData chunkData = new ChunkData(settings);
            world.AddComponent(chunkData);		
            return world;
        }
	
    }
}
