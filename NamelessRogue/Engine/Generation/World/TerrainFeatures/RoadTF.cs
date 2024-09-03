using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Generation.World.TerrainFeatures
{
    internal class RoadTF : ITerrainFeature
    {
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }
        public string Name => "Road";

        public RoadTF(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }

        public void Draw(Chunk chunkToDrawOn)
        {
            var chunkWorldLocationVector = chunkToDrawOn.ChunkWorldMapLocationPoint.ToVector2();

            Pen asphaultPen = new Pen(System.Drawing.Color.Green, Constants.ChunkSize-8);

            Pen asphaultSidewalkPen = new Pen(System.Drawing.Color.Blue, Constants.ChunkSize);

            float[] dashValues = { 3, 1 };
            Pen paintedAsphaultPen = new Pen(System.Drawing.Color.Red, 2);
            paintedAsphaultPen.DashPattern = dashValues;

            var asphaultBitmap = new Bitmap(Constants.ChunkSize, Constants.ChunkSize);

            var graphicsAsphault = Graphics.FromImage(asphaultBitmap);
            var chHalf = Constants.ChunkSize / 2;
            var halfV = new Microsoft.Xna.Framework.Vector2(chHalf);
            Microsoft.Xna.Framework.Point ScalePoint(Vector2 p)
            {
                return ((p * Constants.ChunkSize) + halfV).ToPoint();
            }

            graphicsAsphault.DrawLine(asphaultSidewalkPen, ScalePoint(Start - chunkWorldLocationVector).ToPoint(), ScalePoint(End - chunkWorldLocationVector).ToPoint());
            graphicsAsphault.DrawLine(asphaultPen, ScalePoint(Start - chunkWorldLocationVector).ToPoint(), ScalePoint(End - chunkWorldLocationVector).ToPoint());
            graphicsAsphault.DrawLine(paintedAsphaultPen, ScalePoint(Start - chunkWorldLocationVector).ToPoint(), ScalePoint(End - chunkWorldLocationVector).ToPoint());

            //then we use the bitmap and fill the chunk
            for (int x = 0; x < Constants.ChunkSize; x++)
            {
                for (int y = 0; y < Constants.ChunkSize; y++)
                {
                    if (asphaultBitmap.GetPixel(x, y).G > 0 && chunkToDrawOn.ChunkTiles[x][y][0].Terrain != TerrainTypes.PaintedAsphault)
                    {
                        chunkToDrawOn.ChunkTiles[x][y][0].Biome = Biomes.None;
                        chunkToDrawOn.ChunkTiles[x][y][0].Terrain = TerrainTypes.AsphaultPoor;
                    }
                    if (asphaultBitmap.GetPixel(x, y).R > 0)
                    {
                        chunkToDrawOn.ChunkTiles[x][y][0].Biome = Biomes.None;
                        chunkToDrawOn.ChunkTiles[x][y][0].Terrain = TerrainTypes.PaintedAsphault;
                    }
                    if (asphaultBitmap.GetPixel(x, y).B > 0 && chunkToDrawOn.ChunkTiles[x][y][0].Terrain== TerrainTypes.Grass)
                    {
                        chunkToDrawOn.ChunkTiles[x][y][0].Biome = Biomes.None;
                        chunkToDrawOn.ChunkTiles[x][y][0].Terrain = TerrainTypes.SidewalkPoor;
                    }
                }
            }
        }
    }
}
