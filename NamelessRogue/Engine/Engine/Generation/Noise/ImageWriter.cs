using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.shell;
using SharpDX.Direct2D1;
using SharpDX.WIC;
using Bitmap = SharpDX.WIC.Bitmap;
using Color = NamelessRogue.Engine.Engine.Utility.Color;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;

namespace NamelessRogue.Engine.Engine.Generation.Noise
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

            Texture2D tex = new Texture2D(NamelessGame.DebugDevice, resolution, resolution, false, SurfaceFormat.Color);
            tex.SetData(arrBytes);
            using (var str = File.OpenWrite(path))
            {
                tex.SaveAsPng(str,resolution,resolution);
            }
            tex.Dispose();






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

            Texture2D tex = new Texture2D(NamelessGame.DebugDevice, resolution, resolution, false, SurfaceFormat.Color);
            tex.SetData(arrBytes);
            using (var str = File.OpenWrite(path))
            {
                tex.SaveAsPng(str, resolution, resolution);
            }
            tex.Dispose();
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
                        
                        var vector = new Vector4(color.getRed(), color.getGreen(), color.getBlue(), 1);
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

            Texture2D tex = new Texture2D(NamelessGame.DebugDevice, resolution, resolution, false, SurfaceFormat.Color);
            tex.SetData(arrBytes);
            using (var str = File.OpenWrite(path))
            {
                tex.SaveAsPng(str, resolution, resolution);
            }
            tex.Dispose();






        }



    }
}