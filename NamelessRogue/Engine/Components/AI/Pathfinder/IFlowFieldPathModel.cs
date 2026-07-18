using Microsoft.Xna.Framework;

namespace NamelessRogue.Engine.Components.AI.Pathfinder
{
    internal interface IPathModel
    {
        bool IsCalculated { get; }
        void CalculateTo(Point to);
        bool GetNextPoint(Point from, out Point? nextPoint);
        void PaintWith(FlowFieldDirection direction);
    }
}