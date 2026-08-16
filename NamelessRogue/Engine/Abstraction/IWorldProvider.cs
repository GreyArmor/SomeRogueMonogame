using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Utility;
using SharpDX.Direct2D1.Effects;
using Tile = NamelessRogue.Engine.Components.ChunksAndTiles.Tile;

namespace NamelessRogue.Engine.Abstraction
{
    public interface IWorldProvider
    {
        int ChunkResolution { get; }
        Tile GetTile(int x, int y, int z);
        bool SetTile(int x, int y, int z, Tile tile);
        Dictionary<Point, Chunk> GetRealityBubbleChunks();
        Dictionary<Point, Chunk> GetChunks();
        List<Chunk> RealityChunks { get; }

        /// <summary>
        /// returns true if successful;
        /// </summary>
        bool MoveEntity(IEntity entity, int x, int y, int z);
        bool MoveEntity(IEntity entity, Vector3Int moveTo);

        bool MoveEntityIgnoreCharacters(IEntity entity, int x, int y, int z);

        //swapped entity is null if no entity was swapped
        bool MoveEntitySwapCharacters(IEntity entity, int x, int y, int z, out IEntity swappedEntity);
        bool MoveEntitySwapCharacters(IEntity entity, Vector3Int moveTo, out IEntity swappedEntity);
        void AddEntityToNewLocation(IEntity entity, int x, int y, int z);
    }
}
