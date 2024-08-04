using FlatSharp.Attributes;

using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using NamelessRogue.Engine.Utility;
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
			throw new NotSupportedException("Value type object cant be filled into");
		}

		public static implicit operator Point(PointStorage thisType)
		{
			if (thisType == null) { return default; }
			Point result = new Point();
			result.X = thisType.X;
			result.Y = thisType.Y;
			return result;
		}

		public static implicit operator PointStorage(Point component)
		{
			if (component == null) { return null; }
			PointStorage result = new PointStorage();
			result.FillFrom(component);
			return result;
		}
	}
}

