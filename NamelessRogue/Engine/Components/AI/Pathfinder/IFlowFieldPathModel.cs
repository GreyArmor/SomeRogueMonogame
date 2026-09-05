using Microsoft.Xna.Framework;

namespace NamelessRogue.Engine.Components.AI.Pathfinder
{
    internal interface IPathModel
    {
        Point FinalPoint { get; }
        bool IsCalculated { get; }
        void CalculateTo(Point to);
        void ClearDebug();
        void DrawDebug();
        bool GetNextPoint(Point from, out Point? nextPoint);
        void PaintWith(Direction direction);
    }
}