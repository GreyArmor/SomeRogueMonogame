using System.Collections.Generic;
using Veldrid;

namespace NamelessRogue.Engine.Components.AI.Pathfinder
{
	public static class AllNeighborProviderFlowfield
	{
		private static readonly int[,] neighbors = new int[,]
		{
			{ 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 }, { -1, -1 }, { 1, -1 }, { 1, 1 }, { -1, 1 }
		};

		public static IEnumerable<Point> GetNeighbors(Point tile)
		{
			var result = new Queue<Point>();

			for (var i = 0; i < 8; i++)
			{
				result.Enqueue(new Point(
					x: tile.X + neighbors[i, 0],
					y: tile.Y + neighbors[i, 1]
				));
			}

			return result;
		}

	}

	public static class SharpCornerNeighborProviderFlowfield
	{
		private static readonly int[,] neighbors = new int[,]
		{
			{ 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 }
		};

		public static IEnumerable<Point> GetNeighbors(Point tile)
		{
			var result = new Queue<Point>();

			for (var i = 0; i < 4; i++)
			{
				result.Enqueue(new Point(
					x: tile.X + neighbors[i, 0],
					y: tile.Y + neighbors[i, 1]
				));
			}

			return result;
		}

	}

	public static class DiagonalNeighborProviderFlowfield
	{
		private static readonly int[,] neighbors = new int[,]
		{
			 { -1, -1 }, { 1, -1 }, { 1, 1 }, { -1, 1 }
		};

		public static IEnumerable<Point> GetNeighbors(Point tile)
		{
			var result = new Queue<Point>();

			for (var i = 0; i < 4; i++)
			{
				result.Enqueue(new Point(
					x: tile.X + neighbors[i, 0],
					y: tile.Y + neighbors[i, 1]
				));
			}

			return result;
		}
	}
}
