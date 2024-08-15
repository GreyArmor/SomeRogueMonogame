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
    internal class RoadNoSidewalkTF
    {
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }
        public string Name => "RoadNoSidewalk";

        public RoadNoSidewalkTF(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }

        public void Draw(Chunk chunkToDrawOn)
        {
            var chunkWorldLocationVector = chunkToDrawOn.ChunkWorldMapLocationPoint.ToVector2();

            Pen asphaultPen = new Pen(System.Drawing.Color.Green, Constants.ChunkSize - 4);

            float[] dashValues = { 5, 3 };
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
            graphicsAsphault.DrawLine(asphaultPen, ScalePoint(Start - chunkWorldLocationVector).ToPoint(), ScalePoint(End - chunkWorldLocationVector).ToPoint());
            graphicsAsphault.DrawLine(paintedAsphaultPen, ScalePoint(Start - chunkWorldLocationVector).ToPoint(), ScalePoint(End - chunkWorldLocationVector).ToPoint());

            //then we use the bitmap and fill the chunk
            for (int x = 0; x < Constants.ChunkSize; x++)
            {
                for (int y = 0; y < Constants.ChunkSize; y++)
                {
                    if (asphaultBitmap.GetPixel(x, y).G > 0)
                    {
                        chunkToDrawOn.ChunkTiles[x][y].Biome = Biomes.None;
                        chunkToDrawOn.ChunkTiles[x][y].Terrain = TerrainTypes.Rocks;
                    }
                    if (asphaultBitmap.GetPixel(x, y).R > 0)
                    {
                        chunkToDrawOn.ChunkTiles[x][y].Biome = Biomes.None;
                        chunkToDrawOn.ChunkTiles[x][y].Terrain = TerrainTypes.Snow;
                    }
                }
            }
        }
    }
}
