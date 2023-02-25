using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Serialization;
using NamelessRogue.Engine.Utility;
using SharpDX.DirectWrite;
using BoundingBox = Microsoft.Xna.Framework.BoundingBox;
using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;
using Vector3 = Microsoft.Xna.Framework.Vector3;
namespace NamelessRogue.Engine.Components.ChunksAndTiles
{
    [SkipClassGeneration]
    public class Chunk : IBoundsProvider
    {
        
        public Point ChunkWorldMapLocationPoint { get; set; }
        
        private Point worldPositionBottomLeftCorner;
        private ChunkData chunkContainer;
        
        private Tile[][] chunkTiles;
        
        private Microsoft.Xna.Framework.BoundingBox boundingBox;
        
        private bool isActive;
        private bool loaded = false;

        public Chunk()
        {
        }

        public Chunk(Point bottomLeftCornerWorld, Point chunkWorldMapLocationPoint, ChunkData chunkContainer)
        {
            ChunkWorldMapLocationPoint = chunkWorldMapLocationPoint;
            boundingBox = new BoundingBox(new Vector3(bottomLeftCornerWorld.X, bottomLeftCornerWorld.Y, 0),
               new Vector3(bottomLeftCornerWorld.X + Constants.ChunkSize,bottomLeftCornerWorld.Y + Constants.ChunkSize,0));
            worldPositionBottomLeftCorner = bottomLeftCornerWorld;
            this.chunkContainer = chunkContainer;

            isActive = false;
        }

        public void FillWithTiles(TerrainGenerator generator, TimelineLayer board)
        {

            var surroundingChunksWithRivers = new List<TileForInlandWaterConnectivity>();

            if (ChunkWorldMapLocationPoint.X > 0 && ChunkWorldMapLocationPoint.Y > 0)
            {
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        var tile = board.InlandWaterConnectivity[this.ChunkWorldMapLocationPoint.X + i][this.ChunkWorldMapLocationPoint.Y + j];
                        if (tile.WaterBorderLines.Any())
                        {
                            surroundingChunksWithRivers.Add(tile);
                        }
                    }
                }
            }

            for (int x = 0; x < Constants.ChunkSize; x++)
            {
                for (int y = 0; y < Constants.ChunkSize; y++)
                {
                    chunkTiles[x][y] = generator.GetTileWithoutRiverWater(x + worldPositionBottomLeftCorner.X,
                        y + worldPositionBottomLeftCorner.Y, Constants.ChunkSize);

                    //if (x == 0 || y == 0 || x == Constants.ChunkSize - 1 || y == Constants.ChunkSize - 1)
                    //{
                    //	chunkTiles[x][y].Terrain = TerrainLibrary.Terrains[TerrainTypes.Nothingness];
                    //}
                }
            }






           // if (surroundingChunksWithRivers.Any())
            if(false)
            {
                //TODO this part is really useful for plotting rivers/roads on map, should be moved to its own class
                // the point is: i must generate a chunk sized bitmap to detect which tiles are river tiles and which are not
                // in this bitmap, black pixels are ignored, white ones are river tiles 

                Pen whitePen = new Pen(System.Drawing.Color.White, Constants.ChunkSize);

                var waterBitmap = new Bitmap(Constants.ChunkSize, Constants.ChunkSize);

                var graphics = Graphics.FromImage(waterBitmap);

                foreach (var chunkWithRivers in surroundingChunksWithRivers)
                {
                    var riverLines = chunkWithRivers.WaterBorderLines;

                    foreach (var waterBorderLine in riverLines)
                    {
                        var chunkRiverPoints = waterBorderLine.Points;

                        var chunkPointIndex = chunkRiverPoints.FindIndex(p =>
                            p.X == chunkWithRivers.x && p.Y == chunkWithRivers.y);

                        var chHalf = Constants.ChunkSize / 2;
                        var halfV = new Microsoft.Xna.Framework.Vector2(chHalf);
                        Point ScalePoint(Point p)
                        {
                            return ((p.ToVector2() * Constants.ChunkSize) + halfV).ToPoint();
                        }

                        int curveLenght = 5;
                        int curveHalfLenght = curveLenght / 2;

                        System.Drawing.Point[] curvePoints = null;
                        List<System.Drawing.Point> endPoints = null;
                        if (chunkRiverPoints.Count >= curveLenght)
                        {
                            curvePoints = new System.Drawing.Point[curveLenght];
                            endPoints = new List<System.Drawing.Point>();
                            int rangeStart = 0, rangeEnd = 0;
                            int rangeIndex = chunkPointIndex;
                            if (chunkPointIndex >= curveHalfLenght && chunkPointIndex < chunkRiverPoints.Count - curveHalfLenght)
                            {
                                rangeStart = -curveHalfLenght;
                                rangeEnd = curveHalfLenght;
                            }

                            //this chunk is a first point in this water line
                            else if (chunkPointIndex <= curveHalfLenght)
                            {
                                rangeStart = 0;
                                rangeEnd = curveLenght - 1;

                                rangeIndex = 0;
                                endPoints.Add(ScalePoint(chunkRiverPoints[rangeStart] - ChunkWorldMapLocationPoint).ToPoint());
                            }

                            //this chunk is last
                            else if (chunkPointIndex > chunkRiverPoints.Count - curveHalfLenght - 1)
                            {
                                rangeStart = -curveLenght + 1;
                                rangeEnd = 0;

                                rangeIndex = chunkRiverPoints.Count-1;

                                endPoints.Add(ScalePoint(chunkRiverPoints[rangeIndex] - ChunkWorldMapLocationPoint).ToPoint());
                            }


                            for (int i = rangeStart, pointsIndex = 0; i <= rangeEnd; i++, pointsIndex++)
                            {
                                curvePoints[pointsIndex] = ScalePoint(chunkRiverPoints[rangeIndex + i] - ChunkWorldMapLocationPoint).ToPoint();
                            }
                        }
                        else
                        {
                            var allpointsCount = chunkRiverPoints.Count;
                            curvePoints = new System.Drawing.Point[allpointsCount];

                            for (int i = 0; i < chunkRiverPoints.Count; i++)
                            {
                                curvePoints[i] = ScalePoint(chunkRiverPoints[i] - ChunkWorldMapLocationPoint).ToPoint();
                            }

                        }


                        graphics.DrawCurve(whitePen, curvePoints);
                        if (endPoints != null)
                        {
                            foreach (var point in endPoints)
                            {
                                graphics.FillEllipse(Brushes.White, point.X - chHalf, point.Y - chHalf, Constants.ChunkSize, Constants.ChunkSize);
                            }
                        }
                    }

                    //then we use the bitmap and fill the chunk
                    for (int x = 0; x < Constants.ChunkSize; x++)
                    {
                        for (int y = 0; y < Constants.ChunkSize; y++)
                        {
                            if (waterBitmap.GetPixel(x, y).R > 0)
                            {
                                chunkTiles[x][y].Biome = Biomes.River;
                                chunkTiles[x][y].Terrain = TerrainTypes.Water;
                            }
                        }
                    }
                }

            }
        }

        public void FillWithDebugTiles(TerrainGenerator generator)
        {

            for (int x = 0; x < Constants.ChunkSize; x++)
            {
                for (int y = 0; y < Constants.ChunkSize; y++)
                {
                    chunkTiles[x][y] = new Tile(TerrainTypes.HardRocks, Biomes.Mountain,
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
            return x >= boundingBox.Min.X && x < boundingBox.Max.X && y >= boundingBox.Min.Y && y < boundingBox.Max.Y;
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
                    FillWithTiles(chunkContainer.GetWorldGenerator(),chunkContainer.WorldBoard);
                    this.JustCreated = true;
                }
                loaded = true;
            }

        }

        public bool JustCreated { get; set; }
		public Point WorldPositionBottomLeftCorner { get => worldPositionBottomLeftCorner; set => worldPositionBottomLeftCorner = value; }
		public BoundingBox Bounds { get => boundingBox; set => boundingBox = value; }
		public ChunkData ChunkContainer { get => chunkContainer; set => chunkContainer = value; }
		public bool Loaded { get => loaded; set => loaded = value; }
		public bool IsActive { get => isActive; set => isActive = value; }

		//TODO: serialization
		private bool LoadFromDisk()
        {
            return false;
        }

        public void Deactivate()
        {
            //todo: do not unload chunks for now, just deactivate

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
            SetTileLocal(localX, localY, tile);
		}

        public void SetTileLocal(int localX, int localY, Tile tile)
        {
			chunkTiles[localX][localY] = tile;
		}

		public Tile GetTileLocal(int localX, int localY)
		{
			return chunkTiles[localX][localY];
		}

	}
}
