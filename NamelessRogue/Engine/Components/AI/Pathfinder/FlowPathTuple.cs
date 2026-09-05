using Microsoft.Xna.Framework;

namespace NamelessRogue.Engine.Components.AI.Pathfinder
{
    internal class FlowPathTuple : IPathModel
	{
		public FlowFieldPathModel ShortPath { get; set; }
        public FlowFieldPathModel FullPath { get; set; }

        public bool IsCalculated => FullPath.IsCalculated;

        public Point FinalPoint { get { if(FullPath.IsCalculated) { return FullPath.FinalPoint; } else { return ShortPath.FinalPoint; } } }

        public void CalculateTo(Point to)
        {
           // throw new System.NotImplementedException();
        }

		public void ClearDebug()
		{
			FullPath.ClearDebug();
		}

        public void DrawDebug()
        {
            FullPath.DrawDebug();
        }

        public bool GetNextPoint(Point from, out Point? nextPoint)
        {

            if (FullPath.IsCalculated)
            {
                return FullPath.GetNextPoint(from, out nextPoint);
            }
            else
            {
                return ShortPath.GetNextPoint(from, out nextPoint);
            }
        }

        public void PaintWith(Direction direction)
        {
           FullPath.PaintWith(direction);
        }
    }
}
