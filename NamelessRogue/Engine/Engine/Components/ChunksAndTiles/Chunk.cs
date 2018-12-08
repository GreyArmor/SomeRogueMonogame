using System;
using System.IO;
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
        private Point worldPositionBottomLeftCorner;
        private ChunkData chunkContainer;

        private Tile[,] chunkTiles;
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


        public Chunk(Point bottomLeftCornerWorld, ChunkData chunkContainer)
        {
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
                    chunkTiles[x,y] = generator.GetTile(x + worldPositionBottomLeftCorner.X,
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
                    chunkTiles[x,y] = new Tile(TerrainTypes.HardRocks,Biomes.Mountain,
                        new Point(x + worldPositionBottomLeftCorner.X, y + worldPositionBottomLeftCorner.Y));
                }
            }
        }

        public Tile[,] GetChunkTiles()
        {
            return chunkTiles;
        }

        public void SetChunkTiles(Tile[,] chunkTiles)
        {
            this.chunkTiles = chunkTiles;
        }

        public bool IsPointInside(Point p)
        {
            return IsPointInside(p.X, p.Y);
        }

        public bool IsPointInside(int x, int y)
        {
            return boundingBox.isPointInside(x, y);
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

            return chunkTiles[localX,localY];
        }

        public void Activate()
        {
           // System.out.print("Activate\n");
            isActive = true;
            if (!loaded)
            {
                chunkTiles = new Tile[Constants.ChunkSize,Constants.ChunkSize];
                if (!LoadFromDisk())
                {
                    FillWithTiles(chunkContainer.GetWorldGenerator());
                }

                loaded = true;
            }

        }

//TODO: serialization
        private bool LoadFromDisk()
        {

            //String appPath = "";
            //try
            //{
            //    appPath = new File(EntryPoint.getProtectionDomain().getCodeSource().getLocation().toURI()
            //        .getPath()).getPath();
            //}
            //catch (Exception e)
            //{
            //    e.printStackTrace();
            //}

            //Tile[,] tiles = SaveManager.LoadChunk(appPath + "\\Chunks",
            //    String.valueOf(worldPositionBottomLeftCorner.Y) + "_" +
            //    String.valueOf(worldPositionBottomLeftCorner.X));
            //if (tiles != null)
            //{

            //    chunkTiles = tiles;
            //    System.out.print("LoadFromDisk true\n");
            //    return true;
            //}

            //System.out.print("LoadFromDisk false\n");
            return false;
        }

        public void Deactivate()
        {
            //todo: do not unlod chunks for now, just deactivate
            /*
            String appPath = "";
            try {
                appPath = new File(EntryPoint.getProtectionDomain().getCodeSource().getLocation().toURI().getPath()).getPath();
            }catch (URISyntaxException e) {
                e.printStackTrace();
            }
            System.out.print("Deactivate\n");
            SaveManager.SaveChunk(appPath+"\\Chunks",this,
                    String.valueOf(worldPositionBottomLeftCorner.Y) + "_" + String.valueOf(worldPositionBottomLeftCorner.X));
    */
            isActive = false;
            //	chunkTiles = null;
        }

        public void SetTile(int x, int y, Tile tile)
        {
            int bottomLeftX = worldPositionBottomLeftCorner.X;
            int bottomLeftY = worldPositionBottomLeftCorner.Y;

            int localX = Math.Abs(bottomLeftX - x);
            int localY = Math.Abs(bottomLeftY - y);

            chunkTiles[localX,localY] = tile;
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
