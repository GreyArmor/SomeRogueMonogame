#region Assembly MonoGame.Framework, Version=3.8.2.1105, Culture=neutral, PublicKeyToken=null
// C:\Users\user\.nuget\packages\monogame.framework.windowsdx\3.8.2.1105\lib\net8.0-windows7.0\MonoGame.Framework.dll
// Decompiled with ICSharpCode.Decompiler 8.1.1.7464
#endregion

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;


namespace NamelessRogue.Engine.Utility
{

    //
    // Summary:
    //    Vector 3 but with integers instead of floats, useful for working with arrays
    [DataContract]
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct Vector3Int : IEquatable<Vector3Int>
    {
        private static readonly Vector3Int zero = new Vector3Int(0, 0, 0);

        private static readonly Vector3Int one = new Vector3Int(1, 1, 1);

        private static readonly Vector3Int unitX = new Vector3Int(1, 0, 0);

        private static readonly Vector3Int unitY = new Vector3Int(0, 1, 0);

        private static readonly Vector3Int unitZ = new Vector3Int(0, 0, 1);

        private static readonly Vector3Int up = new Vector3Int(0, 1, 0);

        private static readonly Vector3Int down = new Vector3Int(0, -1, 0);

        private static readonly Vector3Int right = new Vector3Int(1, 0, 0);

        private static readonly Vector3Int left = new Vector3Int(-1, 0, 0);

        private static readonly Vector3Int forward = new Vector3Int(0, 0, -1);

        private static readonly Vector3Int backward = new Vector3Int(0, 0, 1);

        //
        // Summary:
        //     The x coordinate of this Microsoft.Xna.Framework.Vector3Int.
        [DataMember]
        public int X;

        //
        // Summary:
        //     The y coordinate of this Microsoft.Xna.Framework.Vector3Int.
        [DataMember]
        public int Y;

        //
        // Summary:
        //     The z coordinate of this Microsoft.Xna.Framework.Vector3Int.
        [DataMember]
        public int Z;

        //
        // Summary:
        //     Returns a Microsoft.Xna.Framework.Vector3Int with components 0, 0, 0.
        public static Vector3Int Zero => zero;

        //
        // Summary:
        //     Returns a Microsoft.Xna.Framework.Vector3Int with components 1, 1, 1.
        public static Vector3Int One => one;

        //
        // Summary:
        //     Returns a Microsoft.Xna.Framework.Vector3Int with components 1, 0, 0.
        public static Vector3Int UnitX => unitX;

        //
        // Summary:
        //     Returns a Microsoft.Xna.Framework.Vector3Int with components 0, 1, 0.
        public static Vector3Int UnitY => unitY;

        //
        // Summary:
        //     Returns a Microsoft.Xna.Framework.Vector3Int with components 0, 0, 1.
        public static Vector3Int UnitZ => unitZ;

        //
        // Summary:
        //     Returns a Microsoft.Xna.Framework.Vector3Int with components 0, 1, 0.
        public static Vector3Int Up => up;

        //
        // Summary:
        //     Returns a Microsoft.Xna.Framework.Vector3Int with components 0, -1, 0.
        public static Vector3Int Down => down;

        //
        // Summary:
        //     Returns a Microsoft.Xna.Framework.Vector3Int with components 1, 0, 0.
        public static Vector3Int Right => right;

        //
        // Summary:
        //     Returns a Microsoft.Xna.Framework.Vector3Int with components -1, 0, 0.
        public static Vector3Int Left => left;

        //
        // Summary:
        //     Returns a Microsoft.Xna.Framework.Vector3Int with components 0, 0, -1.
        public static Vector3Int Forward => forward;

        //
        // Summary:
        //     Returns a Microsoft.Xna.Framework.Vector3Int with components 0, 0, 1.
        public static Vector3Int Backward => backward;

        internal string DebugDisplayString => X + "  " + Y + "  " + Z;

        //
        // Summary:
        //     Constructs a 3d vector with X, Y and Z from three values.
        //
        // Parameters:
        //   x:
        //     The x coordinate in 3d-space.
        //
        //   y:
        //     The y coordinate in 3d-space.
        //
        //   z:
        //     The z coordinate in 3d-space.
        public Vector3Int(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        //
        // Summary:
        //     Constructs a 3d vector with X, Y and Z set to the same value.
        //
        // Parameters:
        //   value:
        //     The x, y and z coordinates in 3d-space.
        public Vector3Int(float v, int value)
        {
            X = value;
            Y = value;
            Z = value;
        }

        //
        // Summary:
        //     Constructs a 3d vector with X, Y from Microsoft.Xna.Framework.Vector2 and Z from
        //     a scalar.
        //
        // Parameters:
        //   value:
        //     The x and y coordinates in 3d-space.
        //
        //   z:
        //     The z coordinate in 3d-space.
        public Vector3Int(Microsoft.Xna.Framework.Vector2 value, int z)
        {
            X = (int)value.X;
            Y = (int)value.Y;
            Z = z;
        }

        //
        // Summary:
        //     Performs vector addition on value1 and value2.
        //
        // Parameters:
        //   value1:
        //     The first vector to add.
        //
        //   value2:
        //     The second vector to add.
        //
        // Returns:
        //     The result of the vector addition.
        public static Vector3Int Add(Vector3Int value1, Vector3Int value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            value1.Z += value2.Z;
            return value1;
        }

        //
        // Summary:
        //     Performs vector addition on value1 and value2, storing the result of the addition
        //     in result.
        //
        // Parameters:
        //   value1:
        //     The first vector to add.
        //
        //   value2:
        //     The second vector to add.
        //
        //   result:
        //     The result of the vector addition.
        public static void Add(ref Vector3Int value1, ref Vector3Int value2, out Vector3Int result)
        {
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            result.Z = value1.Z + value2.Z;
        }

        public static Vector3Int Clamp(Vector3Int value1, Vector3Int min, Vector3Int max)
        {
            return new Vector3Int(MathHelper.Clamp(value1.X, min.X, max.X), MathHelper.Clamp(value1.Y, min.Y, max.Y), MathHelper.Clamp(value1.Z, min.Z, max.Z));
        }

        //
        // Summary:
        //     Clamps the specified value within a range.
        //
        // Parameters:
        //   value1:
        //     The value to clamp.
        //
        //   min:
        //     The min value.
        //
        //   max:
        //     The max value.
        //
        //   result:
        //     The clamped value as an output parameter.
        public static void Clamp(ref Vector3Int value1, ref Vector3Int min, ref Vector3Int max, out Vector3Int result)
        {
            result.X = MathHelper.Clamp(value1.X, min.X, max.X);
            result.Y = MathHelper.Clamp(value1.Y, min.Y, max.Y);
            result.Z = MathHelper.Clamp(value1.Z, min.Z, max.Z);
        }

        //
        // Summary:
        //     Computes the cross product of two vectors.
        //
        // Parameters:
        //   vector1:
        //     The first vector.
        //
        //   vector2:
        //     The second vector.
        //
        // Returns:
        //     The cross product of two vectors.
        public static Vector3Int Cross(Vector3Int vector1, Vector3Int vector2)
        {
            Cross(ref vector1, ref vector2, out vector1);
            return vector1;
        }

        //
        // Summary:
        //     Computes the cross product of two vectors.
        //
        // Parameters:
        //   vector1:
        //     The first vector.
        //
        //   vector2:
        //     The second vector.
        //
        //   result:
        //     The cross product of two vectors as an output parameter.
        public static void Cross(ref Vector3Int vector1, ref Vector3Int vector2, out Vector3Int result)
        {
            int x = vector1.Y * vector2.Z - vector2.Y * vector1.Z;
            int y = 0 - (vector1.X * vector2.Z - vector2.X * vector1.Z);
            int z = vector1.X * vector2.Y - vector2.X * vector1.Y;
            result.X = x;
            result.Y = y;
            result.Z = z;
        }

        //
        // Summary:
        //     Returns the distance between two vectors, rounded down.
        //
        // Parameters:
        //   value1:
        //     The first vector.
        //
        //   value2:
        //     The second vector.
        //
        // Returns:
        //     The distance between two vectors.
        public static int Distance(Vector3Int value1, Vector3Int value2)
        {
            DistanceSquared(ref value1, ref value2, out var result);
            return (int)MathF.Sqrt(result);
        }

        //
        // Summary:
        //     Returns the distance between two vectors, rounded down.
        //
        // Parameters:
        //   value1:
        //     The first vector.
        //
        //   value2:
        //     The second vector.
        //
        //   result:
        //     The distance between two vectors as an output parameter.
        public static void Distance(ref Vector3Int value1, ref Vector3Int value2, out int result)
        {
            DistanceSquared(ref value1, ref value2, out result);
            result = (int)MathF.Sqrt(result);
        }

        //
        // Summary:
        //     Returns the squared distance between two vectors.
        //
        // Parameters:
        //   value1:
        //     The first vector.
        //
        //   value2:
        //     The second vector.
        //
        // Returns:
        //     The squared distance between two vectors.
        public static int DistanceSquared(Vector3Int value1, Vector3Int value2)
        {
            return (value1.X - value2.X) * (value1.X - value2.X) + (value1.Y - value2.Y) * (value1.Y - value2.Y) + (value1.Z - value2.Z) * (value1.Z - value2.Z);
        }

        //
        // Summary:
        //     Returns the squared distance between two vectors.
        //
        // Parameters:
        //   value1:
        //     The first vector.
        //
        //   value2:
        //     The second vector.
        //
        //   result:
        //     The squared distance between two vectors as an output parameter.
        public static void DistanceSquared(ref Vector3Int value1, ref Vector3Int value2, out int result)
        {
            result = (value1.X - value2.X) * (value1.X - value2.X) + (value1.Y - value2.Y) * (value1.Y - value2.Y) + (value1.Z - value2.Z) * (value1.Z - value2.Z);
        }

        //
        // Summary:
        //     Divides the components of a Microsoft.Xna.Framework.Vector3Int by the components
        //     of another Microsoft.Xna.Framework.Vector3Int.
        //
        // Parameters:
        //   value1:
        //     Source Microsoft.Xna.Framework.Vector3Int.
        //
        //   value2:
        //     Divisor Microsoft.Xna.Framework.Vector3Int.
        //
        // Returns:
        //     The result of dividing the vectors.
        public static Vector3Int Divide(Vector3Int value1, Vector3Int value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            value1.Z /= value2.Z;
            return value1;
        }

        //
        // Summary:
        //     Divides the components of a Microsoft.Xna.Framework.Vector3Int by a scalar.
        //
        // Parameters:
        //   value1:
        //     Source Microsoft.Xna.Framework.Vector3Int.
        //
        //   divider:
        //     Divisor scalar.
        //
        // Returns:
        //     The result of dividing a vector by a scalar.
        public static Vector3Int Divide(Vector3Int value1, int divider)
        {
            int num = 1 / divider;
            value1.X *= num;
            value1.Y *= num;
            value1.Z *= num;
            return value1;
        }

        //
        // Summary:
        //     Divides the components of a Microsoft.Xna.Framework.Vector3Int by a scalar.
        //
        // Parameters:
        //   value1:
        //     Source Microsoft.Xna.Framework.Vector3Int.
        //
        //   divider:
        //     Divisor scalar.
        //
        //   result:
        //     The result of dividing a vector by a scalar as an output parameter.
        public static void Divide(ref Vector3Int value1, int divider, out Vector3Int result)
        {
            int num = 1 / divider;
            result.X = value1.X * num;
            result.Y = value1.Y * num;
            result.Z = value1.Z * num;
        }

        //
        // Summary:
        //     Divides the components of a Microsoft.Xna.Framework.Vector3Int by the components
        //     of another Microsoft.Xna.Framework.Vector3Int.
        //
        // Parameters:
        //   value1:
        //     Source Microsoft.Xna.Framework.Vector3Int.
        //
        //   value2:
        //     Divisor Microsoft.Xna.Framework.Vector3Int.
        //
        //   result:
        //     The result of dividing the vectors as an output parameter.
        public static void Divide(ref Vector3Int value1, ref Vector3Int value2, out Vector3Int result)
        {
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
            result.Z = value1.Z / value2.Z;
        }

        //
        // Summary:
        //     Returns a dot product of two vectors.
        //
        // Parameters:
        //   value1:
        //     The first vector.
        //
        //   value2:
        //     The second vector.
        //
        // Returns:
        //     The dot product of two vectors.
        public static int Dot(Vector3Int value1, Vector3Int value2)
        {
            return value1.X * value2.X + value1.Y * value2.Y + value1.Z * value2.Z;
        }

        //
        // Summary:
        //     Returns a dot product of two vectors.
        //
        // Parameters:
        //   value1:
        //     The first vector.
        //
        //   value2:
        //     The second vector.
        //
        //   result:
        //     The dot product of two vectors as an output parameter.
        public static void Dot(ref Vector3Int value1, ref Vector3Int value2, out int result)
        {
            result = value1.X * value2.X + value1.Y * value2.Y + value1.Z * value2.Z;
        }

        //
        // Summary:
        //     Compares whether current instance is equal to specified System.Object.
        //
        // Parameters:
        //   obj:
        //     The System.Object to compare.
        //
        // Returns:
        //     true if the instances are equal; false otherwise.
        public override bool Equals(object obj)
        {
            if (!(obj is Vector3Int vector))
            {
                return false;
            }

            if (X == vector.X && Y == vector.Y)
            {
                return Z == vector.Z;
            }

            return false;
        }

        //
        // Summary:
        //     Compares whether current instance is equal to specified Microsoft.Xna.Framework.Vector3Int.
        //
        //
        // Parameters:
        //   other:
        //     The Microsoft.Xna.Framework.Vector3Int to compare.
        //
        // Returns:
        //     true if the instances are equal; false otherwise.
        public bool Equals(Vector3Int other)
        {
            if (X == other.X && Y == other.Y)
            {
                return Z == other.Z;
            }

            return false;
        }

        //
        // Summary:
        //     Gets the hash code of this Microsoft.Xna.Framework.Vector3Int.
        //
        // Returns:
        //     Hash code of this Microsoft.Xna.Framework.Vector3Int.
        public override int GetHashCode()
        {
            return (((X.GetHashCode() * 397) ^ Y.GetHashCode()) * 397) ^ Z.GetHashCode();
        }



        //
        // Summary:
        //     Returns the length of this Microsoft.Xna.Framework.Vector3Int.
        //
        // Returns:
        //     The length of this Microsoft.Xna.Framework.Vector3Int.
        public int Length()
        {
            return (int)MathF.Sqrt(X * X + Y * Y + Z * Z);
        }

        //
        // Summary:
        //     Returns the squared length of this Microsoft.Xna.Framework.Vector3Int.
        //
        // Returns:
        //     The squared length of this Microsoft.Xna.Framework.Vector3Int.
        public int LengthSquared()
        {
            return X * X + Y * Y + Z * Z;
        }



        //
        // Summary:
        //     Creates a new Microsoft.Xna.Framework.Vector3Int that contains a maximal values
        //     from the two vectors.
        //
        // Parameters:
        //   value1:
        //     The first vector.
        //
        //   value2:
        //     The second vector.
        //
        // Returns:
        //     The Microsoft.Xna.Framework.Vector3Int with maximal values from the two vectors.
        public static Vector3Int Max(Vector3Int value1, Vector3Int value2)
        {
            return new Vector3Int(MathHelper.Max(value1.X, value2.X), MathHelper.Max(value1.Y, value2.Y), MathHelper.Max(value1.Z, value2.Z));
        }

        //
        // Summary:
        //     Creates a new Microsoft.Xna.Framework.Vector3Int that contains a maximal values
        //     from the two vectors.
        //
        // Parameters:
        //   value1:
        //     The first vector.
        //
        //   value2:
        //     The second vector.
        //
        //   result:
        //     The Microsoft.Xna.Framework.Vector3Int with maximal values from the two vectors
        //     as an output parameter.
        public static void Max(ref Vector3Int value1, ref Vector3Int value2, out Vector3Int result)
        {
            result.X = MathHelper.Max(value1.X, value2.X);
            result.Y = MathHelper.Max(value1.Y, value2.Y);
            result.Z = MathHelper.Max(value1.Z, value2.Z);
        }

        //
        // Summary:
        //     Creates a new Microsoft.Xna.Framework.Vector3Int that contains a minimal values
        //     from the two vectors.
        //
        // Parameters:
        //   value1:
        //     The first vector.
        //
        //   value2:
        //     The second vector.
        //
        // Returns:
        //     The Microsoft.Xna.Framework.Vector3Int with minimal values from the two vectors.
        public static Vector3Int Min(Vector3Int value1, Vector3Int value2)
        {
            return new Vector3Int(MathHelper.Min(value1.X, value2.X), MathHelper.Min(value1.Y, value2.Y), MathHelper.Min(value1.Z, value2.Z));
        }

        //
        // Summary:
        //     Creates a new Microsoft.Xna.Framework.Vector3Int that contains a minimal values
        //     from the two vectors.
        //
        // Parameters:
        //   value1:
        //     The first vector.
        //
        //   value2:
        //     The second vector.
        //
        //   result:
        //     The Microsoft.Xna.Framework.Vector3Int with minimal values from the two vectors
        //     as an output parameter.
        public static void Min(ref Vector3Int value1, ref Vector3Int value2, out Vector3Int result)
        {
            result.X = MathHelper.Min(value1.X, value2.X);
            result.Y = MathHelper.Min(value1.Y, value2.Y);
            result.Z = MathHelper.Min(value1.Z, value2.Z);
        }

        //
        // Summary:
        //     Creates a new Microsoft.Xna.Framework.Vector3Int that contains a multiplication
        //     of two vectors.
        //
        // Parameters:
        //   value1:
        //     Source Microsoft.Xna.Framework.Vector3Int.
        //
        //   value2:
        //     Source Microsoft.Xna.Framework.Vector3Int.
        //
        // Returns:
        //     The result of the vector multiplication.
        public static Vector3Int Multiply(Vector3Int value1, Vector3Int value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            value1.Z *= value2.Z;
            return value1;
        }

        //
        // Summary:
        //     Creates a new Microsoft.Xna.Framework.Vector3Int that contains a multiplication
        //     of Microsoft.Xna.Framework.Vector3Int and a scalar.
        //
        // Parameters:
        //   value1:
        //     Source Microsoft.Xna.Framework.Vector3Int.
        //
        //   scaleFactor:
        //     Scalar value.
        //
        // Returns:
        //     The result of the vector multiplication with a scalar.
        public static Vector3Int Multiply(Vector3Int value1, int scaleFactor)
        {
            value1.X *= scaleFactor;
            value1.Y *= scaleFactor;
            value1.Z *= scaleFactor;
            return value1;
        }

        //
        // Summary:
        //     Creates a new Microsoft.Xna.Framework.Vector3Int that contains a multiplication
        //     of Microsoft.Xna.Framework.Vector3Int and a scalar.
        //
        // Parameters:
        //   value1:
        //     Source Microsoft.Xna.Framework.Vector3Int.
        //
        //   scaleFactor:
        //     Scalar value.
        //
        //   result:
        //     The result of the multiplication with a scalar as an output parameter.
        public static void Multiply(ref Vector3Int value1, int scaleFactor, out Vector3Int result)
        {
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
            result.Z = value1.Z * scaleFactor;
        }

        //
        // Summary:
        //     Creates a new Microsoft.Xna.Framework.Vector3Int that contains a multiplication
        //     of two vectors.
        //
        // Parameters:
        //   value1:
        //     Source Microsoft.Xna.Framework.Vector3Int.
        //
        //   value2:
        //     Source Microsoft.Xna.Framework.Vector3Int.
        //
        //   result:
        //     The result of the vector multiplication as an output parameter.
        public static void Multiply(ref Vector3Int value1, ref Vector3Int value2, out Vector3Int result)
        {
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
            result.Z = value1.Z * value2.Z;
        }

        //
        // Summary:
        //     Creates a new Microsoft.Xna.Framework.Vector3Int that contains the specified vector
        //     inversion.
        //
        // Parameters:
        //   value:
        //     Source Microsoft.Xna.Framework.Vector3Int.
        //
        // Returns:
        //     The result of the vector inversion.
        public static Vector3Int Negate(Vector3Int value)
        {
            value = new Vector3Int(0 - value.X, 0 - value.Y, 0 - value.Z);
            return value;
        }

        //
        // Summary:
        //     Creates a new Microsoft.Xna.Framework.Vector3Int that contains the specified vector
        //     inversion.
        //
        // Parameters:
        //   value:
        //     Source Microsoft.Xna.Framework.Vector3Int.
        //
        //   result:
        //     The result of the vector inversion as an output parameter.
        public static void Negate(ref Vector3Int value, out Vector3Int result)
        {
            result.X = 0 - value.X;
            result.Y = 0 - value.Y;
            result.Z = 0 - value.Z;

        }

        //
        // Summary:
        //     Creates a new Microsoft.Xna.Framework.Vector3Int that contains subtraction of on
        //     Microsoft.Xna.Framework.Vector3Int from a another.
        //
        // Parameters:
        //   value1:
        //     Source Microsoft.Xna.Framework.Vector3Int.
        //
        //   value2:
        //     Source Microsoft.Xna.Framework.Vector3Int.
        //
        // Returns:
        //     The result of the vector subtraction.
        public static Vector3Int Subtract(Vector3Int value1, Vector3Int value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            value1.Z -= value2.Z;
            return value1;
        }

        //
        // Summary:
        //     Creates a new Microsoft.Xna.Framework.Vector3Int that contains subtraction of on
        //     Microsoft.Xna.Framework.Vector3Int from a another.
        //
        // Parameters:
        //   value1:
        //     Source Microsoft.Xna.Framework.Vector3Int.
        //
        //   value2:
        //     Source Microsoft.Xna.Framework.Vector3Int.
        //
        //   result:
        //     The result of the vector subtraction as an output parameter.
        public static void Subtract(ref Vector3Int value1, ref Vector3Int value2, out Vector3Int result)
        {
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            result.Z = value1.Z - value2.Z;
        }

        //
        // Summary:
        //     Returns a System.String representation of this Microsoft.Xna.Framework.Vector3Int
        //     in the format: {X:[Microsoft.Xna.Framework.Vector3Int.X] Y:[Microsoft.Xna.Framework.Vector3Int.Y]
        //     Z:[Microsoft.Xna.Framework.Vector3Int.Z]}
        //
        // Returns:
        //     A System.String representation of this Microsoft.Xna.Framework.Vector3Int.
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(32);
            stringBuilder.Append("{X:");
            stringBuilder.Append(X);
            stringBuilder.Append(" Y:");
            stringBuilder.Append(Y);
            stringBuilder.Append(" Z:");
            stringBuilder.Append(Z);
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }




     

  

        //
        // Summary:
        //     Deconstruction method for Microsoft.Xna.Framework.Vector3Int.
        //
        // Parameters:
        //   x:
        //
        //   y:
        //
        //   z:
        public void Deconstruct(out int x, out int y, out int z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        //
        // Summary:
        //     Returns a System.Numerics.Vector3Int.
        public System.Numerics.Vector3 ToNumerics()
        {
            return new System.Numerics.Vector3(X, Y, Z);
        }

        //
        // Summary:
        //     Converts a System.Numerics.Vector3Int to a Microsoft.Xna.Framework.Vector3Int.
        //
        // Parameters:
        //   value:
        //     The converted value.
        public static implicit operator Vector3Int(System.Numerics.Vector3 value)
        {
            return new Vector3Int((int)value.X, (int)value.Y, (int)value.Z);
        }

        //
        // Summary:
        //     Compares whether two Microsoft.Xna.Framework.Vector3Int instances are equal.
        //
        // Parameters:
        //   value1:
        //     Microsoft.Xna.Framework.Vector3Int instance on the left of the equal sign.
        //
        //   value2:
        //     Microsoft.Xna.Framework.Vector3Int instance on the right of the equal sign.
        //
        // Returns:
        //     true if the instances are equal; false otherwise.
        public static bool operator ==(Vector3Int value1, Vector3Int value2)
        {
            if (value1.X == value2.X && value1.Y == value2.Y)
            {
                return value1.Z == value2.Z;
            }

            return false;
        }

        //
        // Summary:
        //     Compares whether two Microsoft.Xna.Framework.Vector3Int instances are not equal.
        //
        //
        // Parameters:
        //   value1:
        //     Microsoft.Xna.Framework.Vector3Int instance on the left of the not equal sign.
        //
        //   value2:
        //     Microsoft.Xna.Framework.Vector3Int instance on the right of the not equal sign.
        //
        //
        // Returns:
        //     true if the instances are not equal; false otherwise.
        public static bool operator !=(Vector3Int value1, Vector3Int value2)
        {
            return !(value1 == value2);
        }

        //
        // Summary:
        //     Adds two vectors.
        //
        // Parameters:
        //   value1:
        //     Source Microsoft.Xna.Framework.Vector3Int on the left of the add sign.
        //
        //   value2:
        //     Source Microsoft.Xna.Framework.Vector3Int on the right of the add sign.
        //
        // Returns:
        //     Sum of the vectors.
        public static Vector3Int operator +(Vector3Int value1, Vector3Int value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            value1.Z += value2.Z;
            return value1;
        }

        //
        // Summary:
        //     Inverts values in the specified Microsoft.Xna.Framework.Vector3Int.
        //
        // Parameters:
        //   value:
        //     Source Microsoft.Xna.Framework.Vector3Int on the right of the sub sign.
        //
        // Returns:
        //     Result of the inversion.
        public static Vector3Int operator -(Vector3Int value)
        {
            value = new Vector3Int(0 - value.X, 0 - value.Y, 0 - value.Z);
            return value;
        }

        //
        // Summary:
        //     Subtracts a Microsoft.Xna.Framework.Vector3Int from a Microsoft.Xna.Framework.Vector3Int.
        //
        //
        // Parameters:
        //   value1:
        //     Source Microsoft.Xna.Framework.Vector3Int on the left of the sub sign.
        //
        //   value2:
        //     Source Microsoft.Xna.Framework.Vector3Int on the right of the sub sign.
        //
        // Returns:
        //     Result of the vector subtraction.
        public static Vector3Int operator -(Vector3Int value1, Vector3Int value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            value1.Z -= value2.Z;
            return value1;
        }

        //
        // Summary:
        //     Multiplies the components of two vectors by each other.
        //
        // Parameters:
        //   value1:
        //     Source Microsoft.Xna.Framework.Vector3Int on the left of the mul sign.
        //
        //   value2:
        //     Source Microsoft.Xna.Framework.Vector3Int on the right of the mul sign.
        //
        // Returns:
        //     Result of the vector multiplication.
        public static Vector3Int operator *(Vector3Int value1, Vector3Int value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            value1.Z *= value2.Z;
            return value1;
        }

        //
        // Summary:
        //     Multiplies the components of vector by a scalar.
        //
        // Parameters:
        //   value:
        //     Source Microsoft.Xna.Framework.Vector3Int on the left of the mul sign.
        //
        //   scaleFactor:
        //     Scalar value on the right of the mul sign.
        //
        // Returns:
        //     Result of the vector multiplication with a scalar.
        public static Vector3Int operator *(Vector3Int value, int scaleFactor)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            value.Z *= scaleFactor;
            return value;
        }

        //
        // Summary:
        //     Multiplies the components of vector by a scalar.
        //
        // Parameters:
        //   scaleFactor:
        //     Scalar value on the left of the mul sign.
        //
        //   value:
        //     Source Microsoft.Xna.Framework.Vector3Int on the right of the mul sign.
        //
        // Returns:
        //     Result of the vector multiplication with a scalar.
        public static Vector3Int operator *(int scaleFactor, Vector3Int value)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            value.Z *= scaleFactor;
            return value;
        }

        //
        // Summary:
        //     Divides the components of a Microsoft.Xna.Framework.Vector3Int by the components
        //     of another Microsoft.Xna.Framework.Vector3Int.
        //
        // Parameters:
        //   value1:
        //     Source Microsoft.Xna.Framework.Vector3Int on the left of the div sign.
        //
        //   value2:
        //     Divisor Microsoft.Xna.Framework.Vector3Int on the right of the div sign.
        //
        // Returns:
        //     The result of dividing the vectors.
        public static Vector3Int operator /(Vector3Int value1, Vector3Int value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            value1.Z /= value2.Z;
            return value1;
        }

        //
        // Summary:
        //     Divides the components of a Microsoft.Xna.Framework.Vector3Int by a scalar.
        //
        // Parameters:
        //   value1:
        //     Source Microsoft.Xna.Framework.Vector3Int on the left of the div sign.
        //
        //   divider:
        //     Divisor scalar on the right of the div sign.
        //
        // Returns:
        //     The result of dividing a vector by a scalar.
        public static Vector3Int operator /(Vector3Int value1, int divider)
        {
            int num = 1 / divider;
            value1.X *= num;
            value1.Y *= num;
            value1.Z *= num;
            return value1;
        }

        public Point ToPoint()
        {
            return new Point(X, Y);
        }
    }
}
