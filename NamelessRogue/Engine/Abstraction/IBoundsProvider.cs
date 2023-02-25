using Microsoft.Xna.Framework;
using BoundingBox = Microsoft.Xna.Framework.BoundingBox;

namespace NamelessRogue.Engine.Abstraction
{
    public interface IBoundsProvider
    {
        BoundingBox Bounds { get; set; }
        bool IsPointInside(Point point);
        bool IsPointInside(int x, int y);
    }
}
