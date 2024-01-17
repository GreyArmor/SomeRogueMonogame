using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace NamelessRogue.Engine.Utility
{
    public class ViewportUtil
    {
        public static Vector3 Unproject(Viewport viewport, Vector3 source, Matrix4x4 projection, Matrix4x4 view, Matrix4x4 world)
        {
            Matrix4x4.Invert(Matrix4x4.Multiply(Matrix4x4.Multiply(world, view), projection), out Matrix4x4 matrix);
            source.X = (((source.X - viewport.X) / ((float)viewport.Width)) * 2f) - 1f;
            source.Y = -((((source.Y - viewport.Y) / ((float)viewport.Height)) * 2f) - 1f);
            source.Z = (source.Z - viewport.MinDepth) / (viewport.MaxDepth - viewport.MinDepth);
            Vector3 vector = Vector3.Transform(source, matrix);
            float a = (((source.X * matrix.M14) + (source.Y * matrix.M24)) + (source.Z * matrix.M34)) + matrix.M44;
            if (!WithinEpsilon(a, 1f))
            {
                vector.X = vector.X / a;
                vector.Y = vector.Y / a;
                vector.Z = vector.Z / a;
            }
            return vector;
        }

        private static bool WithinEpsilon(float a, float b)
        {
            float num = a - b;
            return ((-1.401298E-45f <= num) && (num <= float.Epsilon));
        }

    }
}
