using FlatSharp.Attributes;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.Serialization.CustomSerializationClasses
{
	[FlatBufferTable]
	public class BoundingBoxStorage : IStorage<BoundingBox>
	{

		[FlatBufferItem(0)] public Vector3Storage Max { get; private set; }
		[FlatBufferItem(1)] public Vector3Storage Min { get; private set; }

		public void FillFrom(BoundingBox component)
		{
			Max = component.Max;
			Min = component.Min;
		}

		public void FillTo(BoundingBox component)
		{
			component.Max = Max;
			component.Min = Min;
		}

		public static implicit operator BoundingBox(BoundingBoxStorage thisType)
		{
			BoundingBox result = new BoundingBox();
			thisType.FillTo(result);
			return result;
		}

		public static implicit operator BoundingBoxStorage(BoundingBox component)
		{
			BoundingBoxStorage result = new BoundingBoxStorage();
			result.FillFrom(result);
			return result;
		}

	}
}

