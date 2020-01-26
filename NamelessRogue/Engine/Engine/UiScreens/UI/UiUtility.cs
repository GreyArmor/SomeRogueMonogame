using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;

namespace NamelessRogue.Engine.Engine.UiScreens.UI
{
    public static class UiUtility
    {
        public static void SetColor(this ProgressBar progressBar, GraphicsDevice device, Color color)
        {
            Texture2D tex = new Texture2D(device, 1, progressBar.Height.Value, false,
                SurfaceFormat.Color);
            byte[] arrBytes = new byte[progressBar.Height.Value * 4];
            for (int x = 0; x < progressBar.Height.Value; x++)
            {

                var red = color.R;
                var green = color.G;
                var blue = color.B;
                arrBytes[x * 4] = (byte)red;
                arrBytes[x * 4 + 1] = (byte)green;
                arrBytes[x * 4 + 2] = (byte)blue;
            }

            tex.SetData(arrBytes);
            progressBar.Filler = new ColoredRegion(new TextureRegion(tex), color);
        }
    }
}
