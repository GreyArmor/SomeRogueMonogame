using FlatSharp.Attributes;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.Serialization.CustomSerializationClasses
{
	[FlatBufferTable]
	public class PointStorage : IStorage<Point>
	{
		[FlatBufferItem(0)] public int X { get; set; }
		[FlatBufferItem(1)] public int Y { get; set; }
		public void FillFrom(Point component)
		{
			X = component.X;
			Y = component.Y;
		}

		public void FillTo(Point component)
		{
			component.X = X;
			component.Y = Y;
		}

		public static implicit operator Point(PointStorage thisType)
		{
			Point result = new Point();
			thisType.FillTo(result);
			return result;
		}

		public static implicit operator PointStorage(Point component)
		{
			PointStorage result = new PointStorage();
			result.FillFrom(result);
			return result;
		}
	}
}

