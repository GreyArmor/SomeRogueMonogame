using Microsoft.Xna.Framework;

namespace NamelessRogue.Engine.Components.AI.Pathfinder
{
    internal interface IPathModel
    {
        bool IsCalculated { get; }
        void CalculateTo(Point to);
        Point GetNextPoint(Point from);
        void PaintWith(FlowFieldDirection direction);
    }
}