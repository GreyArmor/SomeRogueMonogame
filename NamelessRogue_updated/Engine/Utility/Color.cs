 

using Microsoft.Xna.Framework;

namespace NamelessRogue.Engine.Utility
{
    public class Color {
        public float Red { get; set; }
        public float Green { get; set; }
        public float Blue { get; set; }
        public float Alpha { get; set; }

        public Color(){}

        public Color(int red255, int green255, int blue255)
        {
            Init(red255/255f, green255/255f, blue255/255f, 0);
        }

        public Color(float red, float green, float blue)
        {
            Init(red, green, blue, 1f);
        }

        public Color(double allColorChannels)
        {
            Init((float)allColorChannels, (float)allColorChannels, (float)allColorChannels, 1);
        }
        public Color(float allColorChannels)
        {
            Init(allColorChannels, allColorChannels, allColorChannels, 1);
        }

        public Color(double red, double green, double blue)
        {
            Init((float) red, (float)green, (float)blue, 1);
        }

        public Color(double red, double green, double blue, double alpha)
        {
            Init((float)red,(float)green,(float)blue, (float)alpha);
        }

        public Color(int red255, int green255, int blue255, int alpha255)
        {
            Init((float)red255 / 255,(float)green255 / 255,(float)blue255 / 255, (float)alpha255 / 255);
        }

        public Color(float red, float green, float blue, float alpha)
        {
            Init(red, green, blue, alpha);
        }

        public Color(Microsoft.Xna.Framework.Color color)
        {
            Init((float)color.R/255, (float)color.G/255, (float)color.B/255, (float)color.A/255);
        }

        private void Init(float red, float green, float blue, float alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public Vector4 ToVector4()
        {
            return new Vector4(Red,Green,Blue,Alpha);
        }

        public System.Drawing.Color ToColor()
        {
            return System.Drawing.Color.FromArgb((int)(Alpha*255), (int)(Red *255), (int)(Green *255), (int)(Blue *255));
        }

        public static Color operator +(Color a, Color b)
        {
            return new Color(a.Red + b.Red, a.Green + b.Green, a.Blue + b.Blue, a.Alpha + b.Alpha);
        }

        public static Color operator -(Color a, Color b)
        {
            return new Color(a.Red + b.Red, a.Green + b.Green, a.Blue + b.Blue, a.Alpha + b.Alpha);
        }

        public static Color operator /(Color a, int divider)
        {
            return new Color(a.Red / divider, a.Green / divider, a.Blue / divider, a.Alpha / divider);
        }

        public static Color operator *(Color a, int multiplier)
        {
            return new Color(a.Red * multiplier, a.Green * multiplier, a.Blue * multiplier, a.Alpha * multiplier);
        }
    }
}
