using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.Components;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using SharpDX.Direct2D1.Effects;
using Tile = NamelessRogue.Engine.Engine.Components.ChunksAndTiles.Tile;

namespace NamelessRogue.Engine.Abstraction
{
    public interface IChunkProvider : IComponent
    {
        Tile getTile(int x, int y);
        bool setTile(int x, int y, Tile tile);
        Dictionary<Point, Chunk> getRealityBubbleChunks();
        Dictionary<Point, Chunk> getChunks();
    }
}
