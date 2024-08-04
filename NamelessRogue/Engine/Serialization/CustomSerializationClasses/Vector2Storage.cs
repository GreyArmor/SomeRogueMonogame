using FlatSharp.Attributes;

using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace NamelessRogue.Engine.Serialization.CustomSerializationClasses
{
	[FlatBufferTable]
	public class Vector2Storage : IStorage<Vector2>
	{
		[FlatBufferItem(0)] public float X { get; set; }
		[FlatBufferItem(1)] public float Y { get; set; }
		public void FillFrom(Vector2 component)
		{
			X = component.X;
			Y = component.Y;
		}

		public void FillTo(Vector2 component)
		{
			throw new NotSupportedException("Value type object cant be filled into");
		}

		public static implicit operator Vector2 (Vector2Storage thisType)
		{
			if (thisType == null) { return default; }
			Vector2 result = new Vector2();
			result.X = thisType.X;
			result.Y = thisType.Y;
			return result;
        }

        public static implicit operator Vector2Storage (Vector2  component)
        {
			if (component == null) { return null; }
			Vector2Storage result = new Vector2Storage();
            result.FillFrom(component);
            return result;
        }

	}
}
