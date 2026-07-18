using Microsoft.Xna.Framework;

namespace NamelessRogue.Engine.Components.AI.Pathfinder
{
    internal class FlowPathTuple : IPathModel
	{
		public FlowFieldPathModel ShortPath { get; set; }
        public FlowFieldPathModel FullPath { get; set; }

        public bool IsCalculated => FullPath.IsCalculated;

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

        public Point GetNextPoint(Point from)
        {

            if (FullPath.IsCalculated)
            {
                return FullPath.GetNextPoint(from);
            }
            else
            {
                return ShortPath.GetNextPoint(from);
            }
        }

        public void PaintWith(FlowFieldDirection direction)
        {
           FullPath.PaintWith(direction);
        }
    }
}
