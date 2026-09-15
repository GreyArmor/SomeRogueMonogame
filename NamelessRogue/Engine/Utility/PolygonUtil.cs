using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Point = Microsoft.Xna.Framework.Point;
namespace NamelessRogue.Engine.Utility;

public class PolygonUtils
{
	public static BoundingBox GetPolygonBoundingBox(System.Numerics.Vector2[] polygonPoints)
	{
		if (polygonPoints == null || polygonPoints.Length == 0)
			throw new ArgumentException("Polygon must have at least one point.");

		float minX = polygonPoints[0].X;
		float maxX = polygonPoints[0].X;
		float minY = polygonPoints[0].Y;
		float maxY = polygonPoints[0].Y;

		for (int i = 1; i < polygonPoints.Length; i++)
		{
			var p = polygonPoints[i];
			if (p.X < minX) minX = p.X;
			if (p.X > maxX) maxX = p.X;
			if (p.Y < minY) minY = p.Y;
			if (p.Y > maxY) maxY = p.Y;
		}

		int iMinX = (int)MathF.Round(minX, MidpointRounding.AwayFromZero);
		int iMinY = (int)MathF.Round(minY, MidpointRounding.AwayFromZero);
		int iMaxX = (int)MathF.Round(maxX, MidpointRounding.AwayFromZero);
		int iMaxY = (int)MathF.Round(maxY, MidpointRounding.AwayFromZero);

		return new BoundingBox(new Point(iMinX, iMinY), new Point(iMaxX, iMaxY));
	}
}


