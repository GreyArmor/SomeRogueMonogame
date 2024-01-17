using Veldrid;
using NamelessRogue.Engine.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Veldrid.Utilities;

namespace NamelessRogue.Engine.Utility
{
	internal class CollisionMath
	{
		public static bool Intersect(Geometry3D geometry3D, Ray ray, out List<int> triangle)
		{
			for (int i = 0; i < geometry3D.Indices.Count; i+=3)
			{
				var idx0 = geometry3D.Indices[i];
				var idx1 = geometry3D.Indices[i+1];
				var idx2 = geometry3D.Indices[i + 2];
				if (Intersect(geometry3D.Vertices[idx0], geometry3D.Vertices[idx1], geometry3D.Vertices[idx2], ray))
				{
					triangle = new List<int>{ i, i+1, i+2 };
					return true;
				}
			}
			triangle = null;
			return false;
		}

		public static bool Intersect(Vector3 p1, Vector3 p2, Vector3 p3, Ray ray)
		{
			// Vectors from p1 to p2/p3 (edges)
			Vector3 e1, e2;

			Vector3 p, q, t;
			float det, invDet, u, v;


			//Find vectors for two edges sharing vertex/point p1
			e1 = p2 - p1;
			e2 = p3 - p1;

			// calculating determinant 
			p = Vector3.Cross(ray.Direction, e2);

			//Calculate determinat
			det = Vector3.Dot(e1, p);

			//if determinant is near zero, ray lies in plane of triangle otherwise not
			if (det > -float.Epsilon && det < float.Epsilon) { return false; }
			invDet = 1.0f / det;

			//calculate distance from p1 to ray origin
			t = ray.Origin - p1;

			//Calculate u parameter
			u = Vector3.Dot(t, p) * invDet;

			//Check for ray intersection
			if (u < 0 || u > 1) { return false; }

			//Prepare to test v parameter
			q = Vector3.Cross(t, e1);

			//Calculate v parameter
			v = Vector3.Dot(ray.Direction, q) * invDet;

			//Check for ray intersection
			if (v < 0 || u + v > 1) { return false; }

			if ((Vector3.Dot(e2, q) * invDet) > float.Epsilon)
			{
				//ray does intersect
				return true;
			}

			// Not at all
			return false;
		}

	}
}
