using System;
using System.Collections.Generic;
using SharpDX;

namespace NamelessRogue.FieldOfView
{
    public class PermissiveVisibility
    {
        /// <param name="blocksLight">A function that accepts the X and Y coordinates of a tile and determines
        /// whether the given tile blocks the passage of light.
        /// </param>
        /// <param name="setVisible">A function that sets a tile to be visible, given its X and Y coordinates.</param>
        /// <param name="getDistance">A function that takes the X and Y coordinate of a point where X >= 0,
        /// Y >= 0, and X >= Y, and returns the distance from the point to the origin (0,0).
        /// </param>
        public PermissiveVisibility(Func<int, int, bool> blocksLight, Action<int, int> setVisible,
            Func<int, int, int> getDistance)
        {
            BlocksLight = blocksLight;
            SetVisible = setVisible;
            GetDistance = getDistance;
        }

        public void Compute(Point origin, int rangeLimit)
        {
            source = new Offset(origin.X, origin.Y);
            this.rangeLimit = rangeLimit;
            for (int q = 0; q < 4; q++)
            {
                quadrant.x = q == 0 || q == 3 ? 1 : -1;
                quadrant.y = q < 2 ? 1 : -1;
                ComputeQuadrant();
            }
        }

        sealed class Bump
        {
            public Bump parent;
            public Offset location;
        }

        struct Field
        {
            public Bump steepBump, shallowBump;
            public Line steep, shallow;
        }

        struct Line
        {
            public Line(Offset near, Offset far) { this.near = near; this.far = far; }

            public Offset near, far;

            public bool isBelow(Offset point)
            {
                return relativeSlope(point) > 0;
            }

            public bool isBelowOrContains(Offset point)
            {
                return relativeSlope(point) >= 0;
            }

            public bool isAbove(Offset point)
            {
                return relativeSlope(point) < 0;
            }

            public bool isAboveOrContains(Offset point)
            {
                return relativeSlope(point) <= 0;
            }

            public bool doesContain(Offset point)
            {
                return relativeSlope(point) == 0;
            }

            // negative if the line is above the point.
            // positive if the line is below the point.
            // 0 if the line is on the point.
            public int relativeSlope(Offset point)
            {
                return (far.y - near.y) * (far.x - point.x) - (far.y - point.y) * (far.x - near.x);
            }
        }

        struct Offset
        {
            public Offset(int x, int y) { this.x = x; this.y = y; }
            public int x, y;
        }

        void ComputeQuadrant()
        {
            const int Infinity = short.MaxValue;
            List<Bump> steepBumps = new List<Bump>(), shallowBumps = new List<Bump>();
            LinkedList<Field> activeFields = new LinkedList<Field>();
            activeFields.AddLast(new Field() { steep = new Line(new Offset(1, 0), new Offset(0, Infinity)), shallow = new Line(new Offset(0, 1), new Offset(Infinity, 0)) });

            Offset dest = new Offset();
            actIsBlocked(dest);
            for (int i = 1; i < Infinity && activeFields.Count != 0; i++)
            {
                LinkedListNode<Field> current = activeFields.First;
                for (int j = 0; j <= i; j++)
                {
                    dest.x = i - j;
                    dest.y = j;
                    current = visitSquare(dest, current, steepBumps, shallowBumps, activeFields);
                }
            }
        }

        bool actIsBlocked(Offset pos)
        {
            if (rangeLimit >= 0 && GetDistance(Math.Max(pos.x, pos.y), Math.Min(pos.x, pos.y)) > rangeLimit) return true;
            int x = pos.x * quadrant.x + source.x, y = pos.y * quadrant.y + source.y;
            SetVisible(x, y);
            return BlocksLight(x, y);
        }

        LinkedListNode<Field> visitSquare(Offset dest, LinkedListNode<Field> currentField, List<Bump> steepBumps, List<Bump> shallowBumps, LinkedList<Field> activeFields)
        {
            Offset topLeft = new Offset(dest.x, dest.y + 1), bottomRight = new Offset(dest.x + 1, dest.y);
            while (currentField != null && currentField.Value.steep.isBelowOrContains(bottomRight)) currentField = currentField.Next;
            if (currentField == null || currentField.Value.shallow.isAboveOrContains(topLeft) || !actIsBlocked(dest)) return currentField;

            if (currentField.Value.shallow.isAbove(bottomRight) && currentField.Value.steep.isBelow(topLeft))
            {
                LinkedListNode<Field> next = currentField.Next;
                activeFields.Remove(currentField);
                return next;
            }
            else if (currentField.Value.shallow.isAbove(bottomRight))
            {
                AddShallowBump(topLeft, currentField, shallowBumps);
                return checkField(currentField, activeFields);
            }
            else if (currentField.Value.steep.isBelow(topLeft))
            {
                AddSteepBump(bottomRight, currentField, steepBumps);
                return checkField(currentField, activeFields);
            }
            else
            {
                LinkedListNode<Field> steeper = currentField, shallower = activeFields.AddBefore(currentField, currentField.Value);
                AddSteepBump(bottomRight, shallower, steepBumps);
                checkField(shallower, activeFields);
                AddShallowBump(topLeft, steeper, shallowBumps);
                return checkField(steeper, activeFields);
            }
        }

        static void AddShallowBump(Offset point, LinkedListNode<Field> currentField, List<Bump> shallowBumps)
        {
            Field value = currentField.Value;
            value.shallow.far = point;
            value.shallowBump = new Bump() { location = point, parent = value.shallowBump };
            shallowBumps.Add(value.shallowBump);

            Bump currentBump = value.steepBump;
            while (currentBump != null)
            {
                if (value.shallow.isAbove(currentBump.location)) value.shallow.near = currentBump.location;
                currentBump = currentBump.parent;
            }
            currentField.Value = value;
        }

        static void AddSteepBump(Offset point, LinkedListNode<Field> currentField, List<Bump> steepBumps)
        {
            Field value = currentField.Value;
            value.steep.far = point;
            value.steepBump = new Bump() { location = point, parent = value.steepBump };
            steepBumps.Add(value.steepBump);
            // Now look through the list of shallow bumps and see if any of them are below the line.
            for (Bump currentBump = value.shallowBump; currentBump != null; currentBump = currentBump.parent)
            {
                if (value.steep.isBelow(currentBump.location)) value.steep.near = currentBump.location;
            }
            currentField.Value = value;
        }

        static LinkedListNode<Field> checkField(LinkedListNode<Field> currentField, LinkedList<Field> activeFields)
        {
            LinkedListNode<Field> result = currentField;
            if (currentField.Value.shallow.doesContain(currentField.Value.steep.near) &&
                currentField.Value.shallow.doesContain(currentField.Value.steep.far) &&
                (currentField.Value.shallow.doesContain(new Offset(0, 1)) || currentField.Value.shallow.doesContain(new Offset(1, 0))))
            {
                result = currentField.Next;
                activeFields.Remove(currentField);
            }
            return result;
        }

        readonly Func<int, int, bool> BlocksLight;
        readonly Func<int, int, int> GetDistance;
        readonly Action<int, int> SetVisible;

        Offset source, quadrant;
        int rangeLimit;
    }
}