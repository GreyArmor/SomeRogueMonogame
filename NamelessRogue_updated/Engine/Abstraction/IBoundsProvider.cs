using Microsoft.Xna.Framework;
using BoundingBox = NamelessRogue.Engine.Engine.Utility.BoundingBox;

namespace NamelessRogue.Engine.Abstraction
{
    public interface IBoundsProvider
    {
        BoundingBox GetBoundingBox();
        void SetBoundingBox(BoundingBox boundingBox);
        bool IsPointInside(Point point);
        bool IsPointInside(int x, int y);
    }
}
