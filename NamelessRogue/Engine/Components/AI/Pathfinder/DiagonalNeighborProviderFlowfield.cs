using Microsoft.Xna.Framework;
using System.Collections.Generic;
namespace NamelessRogue.Engine.Components.AI.Pathfinder
{
	public static class DiagonalNeighborProviderFlowfield
	{
		private static readonly int[,] neighbors = new int[,]
		{
			{ 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 }, { -1, -1 }, { 1, -1 }, { 1, 1 }, { -1, 1 }
		};

		public static IEnumerable<Point> GetNeighbors(Point tile)
		{
			var result = new List<Point>();

			for (var i = 0; i < neighbors.GetLongLength(0); i++)
			{
				result.Add(new Point(
					x: tile.X + neighbors[i, 0],
					y: tile.Y + neighbors[i, 1]
				));
			}

			return result;
		}
	}

}
