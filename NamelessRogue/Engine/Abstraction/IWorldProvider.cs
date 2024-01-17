using System.Collections.Generic;
using Veldrid;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using Tile = NamelessRogue.Engine.Components.ChunksAndTiles.Tile;
using Veldrid;

namespace NamelessRogue.Engine.Abstraction
{
    public interface IWorldProvider
    {
        int ChunkResolution { get; }
        Tile GetTile(int x, int y);
        bool SetTile(int x, int y, Tile tile);
        Dictionary<Point, Chunk> GetRealityBubbleChunks();
        Dictionary<Point, Chunk> GetChunks();
        List<Chunk> RealityChunks { get; }

        /// <summary>
        /// returns true if successful;
        /// </summary>
        bool MoveEntity(IEntity entity, Point moveTo);
		void ClearTheWay(IEntity entity, Point moveTo);
		void AddEntityToNewLocation(IEntity entity, Point moveTo);
	}
}
