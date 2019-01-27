using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.Components;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using SharpDX.Direct2D1.Effects;
using Tile = NamelessRogue.Engine.Engine.Components.ChunksAndTiles.Tile;

namespace NamelessRogue.Engine.Abstraction
{
    public interface IChunkProvider
    {
        int ChunkResolution { get; }
        Tile GetTile(int x, int y);
        bool SetTile(int x, int y, Tile tile);
        Dictionary<Point, Chunk> GetRealityBubbleChunks();
        Dictionary<Point, Chunk> GetChunks();
        List<Chunk> RealityChunks { get; }
    }
}
