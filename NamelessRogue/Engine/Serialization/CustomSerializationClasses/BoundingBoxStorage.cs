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

		[FlatBufferItem(0)] public Vector3Storage Max { get; set; }
		[FlatBufferItem(1)] public Vector3Storage Min { get; set; }

		public void FillFrom(BoundingBox component)
		{
			Max = component.Max;
			Min = component.Min;
		}

	

		public void FillTo(BoundingBox component)
		{
			throw new NotSupportedException("Value type object cant be filled into");
		}		

		public static implicit operator BoundingBox(BoundingBoxStorage thisType)
		{
			BoundingBox result = new BoundingBox();
			result.Max = thisType.Max;
			result.Min = thisType.Min;
			return result;
		}

		public static implicit operator BoundingBoxStorage(BoundingBox component)
		{
			BoundingBoxStorage result = new BoundingBoxStorage();
			 result.FillFrom(component);
			return result;
		}

	}
}

