using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using log4net.Appender;
using RogueSharp.Random;


using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.shell;
using Color = NamelessRogue.Engine.Utility.Color;
using Image = System.Drawing.Image;
using NamelessRogue.Engine.Utility;
using System.Numerics;

namespace NamelessRogue.Engine.Generation.Noise
{
    public class ImageWriter
    {
        //just convenience methods for debug
        public static void TerrainWriteImage(double[,] data, int resolution, string path)
        {
            // this takes and array of doubles between 0 and 1 and generates a grey scale image from them

            var arr = new Vector4[resolution, resolution];

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    if (data[x, y] > 1)
                    {
                        data[x, y] = 1;
                    }

                    if (data[x, y] < 0)
                    {
                        data[x, y] = 0;
                    }

                    Color col = new Color();
                    if (data[x, y] > 0.80)
                    {
                        col = new Color(1f, 1f, 1f, 1f);
                    }
                    else if (data[x, y] > 0.75)
                    {
                        col = new Color(0.3f, 0.3f, 0.3f, 0.5f);
                    }

                    else if (data[x, y] > 0.7)
                    {
                        col = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                    }
                    else if (data[x, y] > 0.65)
                    {
                        col = new Color(0.7f, 0.7f, 0.7f, 0.7f);
                    }
                    else if (data[x, y] > 0.51)
                    {
                        col = new Color(0, 1f, 0, 1f);
                    }
                    else if (data[x, y] >= 0.5)
                    {
                        col = new Color(1f, 1f, 0, 1);
                    }
                    else if (data[x, y] < 0.5)
                    {
                        col = new Color(0, 0f, 1f, 1f);
                    }

                    arr[x, y] = col.ToVector4();
                }
            }

            byte[] arrBytes = new byte[resolution*resolution*4];
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    arrBytes[y * resolution * 4 + x*4]   = (byte) ((byte)arr[x, y].X*255f);
                    arrBytes[y * resolution * 4 + x*4+1] = (byte)((byte)arr[x, y].Y * 255f);
                    arrBytes[y * resolution * 4 + x*4+2] = (byte)((byte)arr[x, y].Z * 255f);
                    arrBytes[y * resolution * 4 + x*4+3] = 255;
                }
            }

            //TextureView tex = new TextureView(NamelessGame.DebugDevice, resolution, resolution, false, SurfaceFormat.Color);
            //tex.SetData(arrBytes);
            //using (var str = File.OpenWrite(path))
            //{
            //    tex.SaveAsPng(str,resolution,resolution);
            //}
            //tex.Dispose();






        }

        public static void RiverBordersWriteImage(List<List<TileForGeneration>> riverBorders, float[][] heights, int resolution, string path)
        {

            var img = new System.Drawing.Bitmap(resolution, resolution, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    var colorRGB = (float) heights[x][y];

                    if (heights[x][y] > 1)
                    {
                        colorRGB = 1;
                    }

                    if (heights[x][y] <= TileNoiseInterpreter.SeaLevelThreshold)
                    {
                        colorRGB = 0;
                    }

                    var col = new Color(colorRGB, colorRGB, colorRGB, 1f);
                    img.SetPixel(x, resolution - 1 - y, col.ToColor());
                }
            }           

            var random = new InternalRandom();
            foreach (var riverBorder in riverBorders)
            {
                var color = new Color(random.Next(0,255), random.Next(0, 255), random.Next(0, 255), 1f);
                foreach (var tileForGeneration in riverBorder)
                {
                    img.SetPixel(tileForGeneration.x, resolution - 1 - tileForGeneration.y, color.ToColor());
                }
            }

            img.Save(path);


            // TextureView tex = new TextureView(NamelessGame.DebugDevice, resolution, resolution, false, SurfaceFormat.Color);
            //tex.SetData(arrBytes);



            //using (var str = File.OpenWrite(path))
            //{
            //    tex.SaveAsPng(str, resolution, resolution);
            //}
            //tex.Dispose();
        }

        public static void WaterWriteImage(bool[][] data, float[][] heights,  int resolution, string path, Color waterColor)
        {

            var img = new System.Drawing.Bitmap(resolution, resolution, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    var colorRGB = (float)heights[x][y];

                    if (heights[x][y] > 1)
                    {
                        colorRGB = 1;
                    }

                    if (heights[x][y] <= TileNoiseInterpreter.SeaLevelThreshold)
                    {
                        colorRGB = 0;
                    }

                    var col = new Color(colorRGB, colorRGB, colorRGB, 1f);

                    if (data[x][y])
                    {
                            img.SetPixel(x, resolution - 1 - y, waterColor.ToColor());
                    }
                    else
                    {
                       img.SetPixel(x, resolution - 1 - y, col.ToColor());
                    }
                }
            }
            img.Save(path);


            // TextureView tex = new TextureView(NamelessGame.DebugDevice, resolution, resolution, false, SurfaceFormat.Color);
            //tex.SetData(arrBytes);



            //using (var str = File.OpenWrite(path))
            //{
            //    tex.SaveAsPng(str, resolution, resolution);
            //}
            //tex.Dispose();
        }

        public static void BiomesWriteImage(double[,] data, int resolution,string path)
        {
            // this takes and array of doubles between 0 and 1 and generates a grey scale image from them

            var arr = new Vector4[resolution, resolution];

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    if (data[x, y] > 1)
                    {
                        data[x, y] = 1;
                    }

                    if (data[x, y] < 0)
                    {
                        data[x, y] = 0;
                    }

                    Color col = new Color();
                    if (data[x, y] > 0.80)
                    {
                        col = new Color(0f, 5f, 0f, 1f);
                    }
                    else {
                        col = new Color(0,0,0,0);
                    }

                    arr[x, y] = col.ToVector4();
                }
            }

            byte[] arrBytes = new byte[resolution * resolution * 4];
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    arrBytes[y * resolution * 4 + x * 4] = (byte)((byte)arr[x, y].X * 255f);
                    arrBytes[y * resolution * 4 + x * 4 + 1] = (byte)((byte)arr[x, y].Y * 255f);
                    arrBytes[y * resolution * 4 + x * 4 + 2] = (byte)((byte)arr[x, y].Z * 255f);
                    arrBytes[y * resolution * 4 + x * 4 + 3] = 255;
                }
            }

            //TextureView tex = new TextureView(NamelessGame.DebugDevice, resolution, resolution, false, SurfaceFormat.Color);
            //tex.SetData(arrBytes);
            //using (var str = File.OpenWrite(path))
            //{
            //    tex.SaveAsPng(str, resolution, resolution);
            //}
            //tex.Dispose();
        }


        public static void RegionsWriteImage(WorldTile[,] data, int resolution, string path)
        {

            var arr = new Vector4[resolution, resolution];

            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    if (data[i, j].Continent!=null)
                    {
                        var color = data[i, j].Continent.Color;
                        
                        var vector = new Vector4(color.Red, color.Green, color.Blue, 1);
                        arr[i, j] = vector;
                    }
                    else
                    {
                        arr[i, j] = new Vector4(0, 0, 0, 1);
                    }
                }
            }
           

            byte[] arrBytes = new byte[resolution * resolution * 4];
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float x1, y1, z1;
                    x1= arr[x, y].X * 255f;
                    y1 = arr[x, y].Y * 255f;
                    z1 = arr[x, y].Z * 255f;
                    arrBytes[y * resolution * 4 + x * 4] = (byte)x1;
                    arrBytes[y * resolution * 4 + x * 4 + 1] = (byte)y1;
                    arrBytes[y * resolution * 4 + x * 4 + 2] = (byte)z1;
                    arrBytes[y * resolution * 4 + x * 4 + 3] = 255;
                }
            }

            //TextureView tex = new TextureView(NamelessGame.DebugDevice, resolution, resolution, false, SurfaceFormat.Color);
            //tex.SetData(arrBytes);
            //using (var str = File.OpenWrite(path))
            //{
            //    tex.SaveAsPng(str, resolution, resolution);
            //}
            //tex.Dispose();
        }



    }
}