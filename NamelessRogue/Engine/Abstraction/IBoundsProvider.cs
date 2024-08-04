
using NamelessRogue.Engine.Utility;

namespace NamelessRogue.Engine.Abstraction
{
    public interface IBoundsProvider
    {
        BoundingBox3D Bounds { get; set; }
        bool IsPointInside(Point point);
        bool IsPointInside(int x, int y);
    }
}
