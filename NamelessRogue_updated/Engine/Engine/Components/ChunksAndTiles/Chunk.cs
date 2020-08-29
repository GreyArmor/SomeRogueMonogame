using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.Engine.Engine.Serialization;
using NamelessRogue.Engine.Engine.Utility;
using BoundingBox = NamelessRogue.Engine.Engine.Utility.BoundingBox;
using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;

namespace NamelessRogue.Engine.Engine.Components.ChunksAndTiles
{
    
    public class Chunk : IBoundsProvider
    {
        
        public Point ChunkWorldMapLocationPoint { get; set; }
        
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


        public Chunk(Point bottomLeftCornerWorld, Point chunkWorldMapLocationPoint, ChunkData chunkContainer)
        {
            ChunkWorldMapLocationPoint = chunkWorldMapLocationPoint;
            boundingBox = new BoundingBox(bottomLeftCornerWorld,
                new Point(bottomLeftCornerWorld.X + Constants.ChunkSize,
                    bottomLeftCornerWorld.Y + Constants.ChunkSize));
            worldPositionBottomLeftCorner = bottomLeftCornerWorld;
            this.chunkContainer = chunkContainer;

            isActive = false;
        }


        public void FillWithTiles(TerrainGenerator generator)
        {

            var surroundingChunksWithRivers = new List<TerrainGenerator.TileForInlandWaterConnectivity>();

			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
                    var tile = generator.InlandWaterConnectivity[this.ChunkWorldMapLocationPoint.X + i][this.ChunkWorldMapLocationPoint.Y + j];
                    if (tile.WaterBorderLines.Any())
                    {
                        surroundingChunksWithRivers.Add(tile);
                    }

                }

			}
            //TODO resolve filled chunks later

            var chunkWater = generator.InlandWaterConnectivity[this.ChunkWorldMapLocationPoint.X][this.ChunkWorldMapLocationPoint.Y];

            if (chunkWater.isWater && !chunkWater.WaterBorderLines.Any())
			{
				for (int x = 0; x < Constants.ChunkSize; x++)
				{
					for (int y = 0; y < Constants.ChunkSize; y++)
					{
						chunkTiles[x][y] = new Tile(
							TerrainLibrary.Terrains[TerrainTypes.Water],
							BiomesLibrary.Biomes[Biomes.River],
							new Point(x + worldPositionBottomLeftCorner.X,
								y + worldPositionBottomLeftCorner.Y), 0);
					}
				}
			}
            else
			{
                for (int x = 0; x < Constants.ChunkSize; x++)
                {
                    for (int y = 0; y < Constants.ChunkSize; y++)
                    {
                        chunkTiles[x][y] = generator.GetTileWithoutRiverWater(x + worldPositionBottomLeftCorner.X,
                            y + worldPositionBottomLeftCorner.Y, Constants.ChunkSize);

                        //if (x == 0 || y == 0 || x == Constants.ChunkSize - 1 || y == Constants.ChunkSize - 1)
                        //{
                        //    chunkTiles[x][y].Terrain = TerrainLibrary.Terrains[TerrainTypes.Nothingness];
                        //}
                    }
                }



                


                if (surroundingChunksWithRivers.Any())
                {
                    //TODO this part is really useful for plotting rivers/roads on map, should be moved to its own class
                    // the point is: i must generate a chunk sized bitmap to detect which tiles are river tiles and which are not
                    // in this bitmap, black pixels are ignored, white ones are river tiles 

                    Pen whitePen = new Pen(System.Drawing.Color.White, Constants.ChunkSize);

                    var waterBitmap = new Bitmap(Constants.ChunkSize, Constants.ChunkSize);

                    var graphics = Graphics.FromImage(waterBitmap);

                    foreach(var chunkWithRivers in surroundingChunksWithRivers)
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
                            int halfLenght = curveLenght / 2;

                            System.Drawing.Point[] points = null;

                            if (chunkRiverPoints.Count >= curveLenght)
                            {
                                points = new System.Drawing.Point[curveLenght];
                                int rangeStart = 0, rangeEnd = 0;
                                int rangeIndex = chunkPointIndex;
                                if (chunkPointIndex >= halfLenght && chunkPointIndex < chunkRiverPoints.Count - halfLenght)
                                {
                                    rangeStart = -halfLenght;
                                    rangeEnd = halfLenght;
                                }

                                //this chunk is a first point in this water line
                                else if (chunkPointIndex <= halfLenght)
                                {
                                    rangeStart = 0;
                                    rangeEnd = curveLenght - 1;

                                    rangeIndex = 0;

                                }

                                //this chunk is last
                                else if (chunkPointIndex > chunkRiverPoints.Count - halfLenght - 1)
                                {
                                    rangeStart = -curveLenght + 1;
                                    rangeEnd = 0;

                                    rangeIndex = chunkRiverPoints.Count - 1;
                                }


                                for (int i = rangeStart, pointsIndex = 0; i <= rangeEnd; i++, pointsIndex++)
                                {
                                        points[pointsIndex] = ScalePoint(chunkRiverPoints[rangeIndex + i] - ChunkWorldMapLocationPoint).ToPoint();
                                }
                            }
                            else
                            {
                                var allpointsCount = chunkRiverPoints.Count;
                                points = new System.Drawing.Point[allpointsCount];

								for (int i = 0; i < chunkRiverPoints.Count; i++)
                                {
                                    points[i] = ScalePoint(chunkRiverPoints[i] - ChunkWorldMapLocationPoint).ToPoint();
                                }

                            }


                            graphics.DrawCurve(whitePen, points);

                        }
                        //then we use the bitmap and fill the chunk
                        for (int x = 0; x < Constants.ChunkSize; x++)
                        {
                            for (int y = 0; y < Constants.ChunkSize; y++)
                            {
                                if (waterBitmap.GetPixel(x, y).R > 0)
                                {
                                    chunkTiles[x][y].Biome = BiomesLibrary.Biomes[Biomes.River];
                                    chunkTiles[x][y].Terrain = TerrainLibrary.Terrains[TerrainTypes.Water];
                                }
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
