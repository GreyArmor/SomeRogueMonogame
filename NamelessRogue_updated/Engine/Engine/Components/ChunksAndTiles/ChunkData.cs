using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Generation;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.Engine.Engine.Infrastructure;

namespace NamelessRogue.Engine.Engine.Components.ChunksAndTiles
{
    public class ChunkData : IWorldProvider
    {
        private Dictionary<Point, Chunk> chunks;

        private Dictionary<Point, Chunk> realityBubbleChunks;
        public List<Chunk> RealityChunks { get; set; } = new List<Chunk>();
        private WorldSettings worldSEttings;

        public ChunkData(WorldSettings settings)
        {
            Id = Guid.NewGuid();
            chunks = new Dictionary<Point, Chunk>();
            realityBubbleChunks = new Dictionary<Point, Chunk>();
            worldSEttings = settings;
            initWorld();
        }

        void initWorld()
        {
            int offfset = 0;
            for (int x = offfset; x <= ChunkResolution + offfset; x++)
            {
                for (int y = offfset; y <= ChunkResolution + offfset; y++)
                {
                    CreateChunk(x, y);
                }
            }
        }

        public void CreateChunk(int x, int y)
        {
            Chunk newChunk = new Chunk(new Point(x * Constants.ChunkSize, y * Constants.ChunkSize), new Point(x,y), this);
            //newChunk.FillWithTiles(terrainGenerator);
            chunks.Add(new Point(x, y), newChunk);
        }

        public void RemoveChunk(int x, int y)
        {
            chunks.Remove(new Point(x, y));
        }


        public Tile GetTileAt(Point p)
        {
            return GetTile(p.X, p.Y);
        }

        //TODO: we need to implement quick iteration by using bounding box trees;
        public int ChunkResolution { get; } = 1000;

        public Tile GetTile(int x, int y)
        {
            Chunk chunkOfPoint = null;

            int chunkX = x / Constants.ChunkSize;
            int chunkY = y / Constants.ChunkSize;            

            realityBubbleChunks.TryGetValue(new Point(chunkX, chunkY),out chunkOfPoint);

            if (chunkOfPoint == null)
            {
                return new Tile(TerrainLibrary.Terrains[TerrainTypes.Nothingness],BiomesLibrary.Biomes[Biomes.None], new Point(-1, -1),0.5);
            }


            return chunkOfPoint.GetTile(x, y);
        }
        public bool SetTile(int x, int y, Tile tile)
        {
            Chunk chunkOfPoint = null;

            int chunkX = x / Constants.ChunkSize;
            int chunkY = y / Constants.ChunkSize;


            realityBubbleChunks.TryGetValue(new Point(chunkX, chunkY), out chunkOfPoint);


            if (chunkOfPoint == null)
            {
                return false;
            }

            chunkOfPoint.SetTile(x, y, tile);
            return true;
        }


        public TerrainGenerator GetWorldGenerator()
        {
            return worldSEttings.TerrainGen;
        }

        public Random GetGlobalRandom()
        {
            return worldSEttings.GlobalRandom;
        }

        public Dictionary<Point, Chunk> GetRealityBubbleChunks()
        {
            return realityBubbleChunks;
        }

        public Dictionary<Point, Chunk> GetChunks()
        {
            return chunks;
        }

        Guid Id;


        public Guid GetId()
        {
            return Id;
        }

        public bool MoveEntity(IEntity entity, Point moveTo)
        {
            Position position = entity.GetComponentOfType<Position>();
            if (position != null)
            {
                IWorldProvider worldProvider = this;

                Tile oldTile = worldProvider.GetTile(position.p.X, position.p.Y);
                Tile newTile = worldProvider.GetTile(moveTo.X, moveTo.Y);

                oldTile.RemoveEntity((Entity) entity);
                newTile.AddEntity((Entity) entity);


                position.p.X = (moveTo.X);
                position.p.Y = (moveTo.Y);



                return true;
            }

            return false;
        }
    }
}
