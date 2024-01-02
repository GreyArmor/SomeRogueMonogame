using FlatSharp.Attributes;
using SharpDX;
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
			Max = component.Maximum;
			Min = component.Minimum;
		}

	

		public void FillTo(BoundingBox component)
		{
			throw new NotSupportedException("Value type object cant be filled into");
		}		

		public static implicit operator BoundingBox(BoundingBoxStorage thisType)
		{
			BoundingBox result = new BoundingBox();
			result.Maximum = thisType.Max;
			result.Minimum = thisType.Min;
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

