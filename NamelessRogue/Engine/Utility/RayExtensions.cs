using NamelessRogue.Engine.Components.Physical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid.Utilities;

namespace NamelessRogue.Engine.Utility
{
    public static class RayExtensions
    {
        public static bool Intersects(this Ray ray, Veldrid.Utilities.BoundingBox box, out float distance)
        {
            float? dist = Intersects(ray, box);
            if (dist.HasValue)
            {
                distance = dist.Value;
                return true;
            }
            else
            {
                distance = -1;
                return false;
            }
        }

        public static float? Intersects(this Ray ray, Veldrid.Utilities.BoundingBox box)
        {
            const float Epsilon = 1e-6f;

            float? tMin = null, tMax = null;

            if (Math.Abs(ray.Direction.X) < Epsilon)
            {
                if (ray.Origin.X < box.Min.X || ray.Origin.X > box.Max.X)
                    return null;
            }
            else
            {
                tMin = (box.Min.X - ray.Origin.X) / ray.Direction.X;
                tMax = (box.Max.X - ray.Origin.X) / ray.Direction.X;

                if (tMin > tMax)
                {
                    var temp = tMin;
                    tMin = tMax;
                    tMax = temp;
                }
            }

            if (Math.Abs(ray.Direction.Y) < Epsilon)
            {
                if (ray.Origin.Y < box.Min.Y || ray.Origin.Y > box.Max.Y)
                    return null;
            }
            else
            {
                var tMinY = (box.Min.Y - ray.Origin.Y) / ray.Direction.Y;
                var tMaxY = (box.Max.Y - ray.Origin.Y) / ray.Direction.Y;

                if (tMinY > tMaxY)
                {
                    var temp = tMinY;
                    tMinY = tMaxY;
                    tMaxY = temp;
                }

                if ((tMin.HasValue && tMin > tMaxY) || (tMax.HasValue && tMinY > tMax))
                    return null;

                if (!tMin.HasValue || tMinY > tMin) tMin = tMinY;
                if (!tMax.HasValue || tMaxY < tMax) tMax = tMaxY;
            }

            if (Math.Abs(ray.Direction.Z) < Epsilon)
            {
                if (ray.Origin.Z < box.Min.Z || ray.Origin.Z > box.Max.Z)
                    return null;
            }
            else
            {
                var tMinZ = (box.Min.Z - ray.Origin.Z) / ray.Direction.Z;
                var tMaxZ = (box.Max.Z - ray.Origin.Z) / ray.Direction.Z;

                if (tMinZ > tMaxZ)
                {
                    var temp = tMinZ;
                    tMinZ = tMaxZ;
                    tMaxZ = temp;
                }

                if ((tMin.HasValue && tMin > tMaxZ) || (tMax.HasValue && tMinZ > tMax))
                    return null;

                if (!tMin.HasValue || tMinZ > tMin) tMin = tMinZ;
                if (!tMax.HasValue || tMaxZ < tMax) tMax = tMaxZ;
            }

            // having a positive tMax and a negative tMin means the ray is inside the box
            // we expect the intesection distance to be 0 in that case
            if ((tMin.HasValue && tMin < 0) && tMax > 0) return 0;

            // a negative tMin means that the intersection point is behind the ray's origin
            // we discard these as not hitting the AABB
            if (tMin < 0) return null;

            return tMin;
        }
    }
}
