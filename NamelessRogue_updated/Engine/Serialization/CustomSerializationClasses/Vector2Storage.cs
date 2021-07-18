using FlatSharp.Attributes;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using System;
using System.Collections.Generic;
using System.Text;

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
			component.X = X;
			component.Y = Y;
		}

		public static implicit operator Vector2 (Vector2Storage thisType)
        {
            Vector2 result = new Vector2();
            thisType.FillTo(result);
            return result;
        }

        public static implicit operator Vector2Storage (Vector2  component)
        {
            Vector2Storage result = new Vector2Storage();
            result.FillFrom(result);
            return result;
        }

	}
}
