using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.Engine.Engine.Infrastructure;

namespace NamelessRogue.Engine.Engine.Components.ChunksAndTiles
{
    public class ChunkData : IChunkProvider
    {
        private Dictionary<Point, Chunk> chunks;

        private Dictionary<Point, Chunk> realityBubbleChunks;

        TerrainGenerator terrainGenerator;

        public ChunkData()
        {
            Id = Guid.NewGuid();
            chunks = new Dictionary<Point, Chunk>();

            terrainGenerator = new TerrainGenerator(5);
            realityBubbleChunks = new Dictionary<Point, Chunk>();
            initWorld();
        }

        void initWorld()
        {
            int testWorldSize = 1000;
            int offfset = 0;
            for (int x = offfset; x <= testWorldSize + offfset; x++)
            {
                for (int y = offfset; y <= testWorldSize + offfset; y++)
                {
                    CreateChunk(x, y);
                }
            }
        }

        public void CreateChunk(int x, int y)
        {
            Chunk newChunk = new Chunk(new Point(x * Constants.ChunkSize, y * Constants.ChunkSize), this);
            //newChunk.FillWithTiles(terrainGenerator);
            chunks.Add(new Point(x, y), newChunk);
        }

        public void RemoveChunk(int x, int y)
        {
            chunks.Remove(new Point(x, y));
        }


        public Tile GetTileAt(Point p)
        {
            return getTile(p.X, p.Y);
        }

        //TODO: we need to implement quick iteration by using bounding box trees;
        public Tile getTile(int x, int y)
        {
            Chunk chunkOfPoint = null;
            foreach (Chunk ch in realityBubbleChunks.Values)
            {

                if (ch.IsPointInside(x, y))
                {
                    chunkOfPoint = ch;
                    break;
                }
            }

            if (chunkOfPoint == null)
            {
                return new Tile(TerrainTypes.Nothingness, new Point(-1, -1));
            }

            return chunkOfPoint.getTile(x, y);
        }

        //TODO: we need to implement quick iteration by using bounding box trees;
        public bool setTile(int x, int y, Tile tile)
        {
            Chunk chunkOfPoint = null;
            foreach (Chunk ch in realityBubbleChunks.Values)
            {
                if (ch.IsPointInside(x, y))
                {
                    chunkOfPoint = ch;
                    break;
                }
            }

            if (chunkOfPoint == null)
            {
                return false;
            }

            chunkOfPoint.SetTile(x, y, tile);
            return true;
        }


        public TerrainGenerator getWorldGenerator()
        {
            return terrainGenerator;
        }

        public Dictionary<Point, Chunk> getRealityBubbleChunks()
        {
            return realityBubbleChunks;
        }

        public Dictionary<Point, Chunk> getChunks()
        {
            return chunks;
        }

        Guid Id;


        public Guid GetId()
        {
            return Id;
        }
    }
}
