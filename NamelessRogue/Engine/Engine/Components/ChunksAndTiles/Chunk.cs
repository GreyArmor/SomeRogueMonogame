using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.Engine.Engine.Serialization;
using BoundingBox = NamelessRogue.Engine.Engine.Utility.BoundingBox;

namespace NamelessRogue.Engine.Engine.Components.ChunksAndTiles
{
    
    public class Chunk : IBoundsProvider
    {
        
        public Point ChunkLocationPoint { get; set; }
        
        private Point worldPositionBottomLeftCorner;
        private ChunkData chunkContainer;
        
        private Tile[][] chunkTiles;
        
        private BoundingBox boundingBox;
        
        private bool isActive;
        private bool loaded = false;

        public Chunk()
        {
        }

        public bool IsActive()
        {
            return isActive;
        }

        public Point GetWorldPosition()
        {
            return worldPositionBottomLeftCorner;
        }

        public void SetWorldPosition(Point worldPosition)
        {
            this.worldPositionBottomLeftCorner = worldPosition;
        }


        public Chunk(Point bottomLeftCornerWorld, Point chunkLocationPoint, ChunkData chunkContainer)
        {
            ChunkLocationPoint = chunkLocationPoint;
            boundingBox = new BoundingBox(bottomLeftCornerWorld,
                new Point(bottomLeftCornerWorld.X + Constants.ChunkSize,
                    bottomLeftCornerWorld.Y + Constants.ChunkSize));
            worldPositionBottomLeftCorner = bottomLeftCornerWorld;
            this.chunkContainer = chunkContainer;

            isActive = false;
        }


        public void FillWithTiles(TerrainGenerator generator)
        {

            for (int x = 0; x < Constants.ChunkSize; x++)
            {
                for (int y = 0; y < Constants.ChunkSize; y++)
                {
                    chunkTiles[x][y] = generator.GetTileWithoutWater(x + worldPositionBottomLeftCorner.X,
                        y + worldPositionBottomLeftCorner.Y, Constants.ChunkSize);
                }
            }
        }

        public void FillWithDebugTiles(TerrainGenerator generator)
        {

            for (int x = 0; x < Constants.ChunkSize; x++)
            {
                for (int y = 0; y < Constants.ChunkSize; y++)
                {
                    chunkTiles[x][y] = new Tile(TerrainLibrary.Terrains[TerrainTypes.HardRocks],BiomesLibrary.Biomes[Biomes.Mountain],
                        new Point(x + worldPositionBottomLeftCorner.X, y + worldPositionBottomLeftCorner.Y), 0.5);
                }
            }
        }

        public Tile[][] GetChunkTiles()
        {
            return chunkTiles;
        }

        public void SetChunkTiles(Tile[][] chunkTiles)
        {
            this.chunkTiles = chunkTiles;
        }

        public bool IsPointInside(Point p)
        {
            return IsPointInside(p.X, p.Y);
        }

        public bool IsPointInside(int x, int y)
        {
            return boundingBox.IsPointInside(x, y);
        }

        public Tile GetTile(Point p)
        {
            return GetTile(p.X, p.Y);
        }

        public Tile GetTile(int x, int y)
        {

            if (!isActive)
            {
                Activate();
            }

            int bottomLeftX = worldPositionBottomLeftCorner.X;
            int bottomLeftY = worldPositionBottomLeftCorner.Y;

            int localX = Math.Abs(bottomLeftX - x);
            int localY = Math.Abs(bottomLeftY - y);

            return chunkTiles[localX][localY];
        }

        public void Activate()
        {
           // System.out.print("Activate\n");
            isActive = true;
            if (!loaded)
            {
                chunkTiles = new Tile[Constants.ChunkSize][];
                for (var index = 0; index < chunkTiles.Length; index++)
                {
                     chunkTiles[index] = new Tile[Constants.ChunkSize];
                }

                if (!LoadFromDisk())
                {
                    FillWithTiles(chunkContainer.GetWorldGenerator());
                    this.JustCreated = true;
                }
                loaded = true;
            }

        }

        public bool JustCreated { get; set; }

//TODO: serialization
        private bool LoadFromDisk()
        {
            return false;
        }

        public void Deactivate()
        {
            //todo: do not unlod chunks for now, just deactivate

            //String appPath = System.IO.Directory.GetCurrentDirectory();
            //SaveManager.SaveChunk(appPath + "\\Chunks", this,
            //        worldPositionBottomLeftCorner.Y + "_" + worldPositionBottomLeftCorner.X);

            isActive = false;
            //	chunkTiles = null;
        }

        public void SetTile(int x, int y, Tile tile)
        {
            int bottomLeftX = worldPositionBottomLeftCorner.X;
            int bottomLeftY = worldPositionBottomLeftCorner.Y;

            int localX = Math.Abs(bottomLeftX - x);
            int localY = Math.Abs(bottomLeftY - y);

            chunkTiles[localX][localY] = tile;
        }

        public BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public void SetBoundingBox(BoundingBox boundingBox)
        {
            this.boundingBox = boundingBox;
        }
    }
}
