using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace NamelessRogue.Engine.Utility
{
        public static class BoundingBoxExtensions
        {
            public static Rectangle ToRectangle(this Microsoft.Xna.Framework.BoundingBox box)
            {
                // Calculate dimensions using Min and Max points
                int x = (int)box.Min.X;
                int y = (int)box.Min.Y;
                int width = (int)(box.Max.X - box.Min.X);
                int height = (int)(box.Max.Y - box.Min.Y);

                return new Rectangle(x, y, width, height);
            }
        }
}
