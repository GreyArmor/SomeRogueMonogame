using FlatSharp.Attributes;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using NamelessRogue.Engine.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.Serialization.CustomSerializationClasses
{
	[FlatBufferTable]
	public class BoundingBoxStorage : IStorage<BoundingBox3D>
	{

		[FlatBufferItem(0)] public Vector3Storage Max { get; set; }
		[FlatBufferItem(1)] public Vector3Storage Min { get; set; }

		public void FillFrom(BoundingBox3D component)
		{
			Max = component.Max;
			Min = component.Min;
		}	

		public void FillTo(BoundingBox3D component)
		{
			throw new NotSupportedException("Value type object cant be filled into");
		}

        public static implicit operator BoundingBox3D(BoundingBoxStorage thisType)
		{
            BoundingBox3D result = new BoundingBox3D();
			result.Max = thisType.Max;
			result.Min = thisType.Min;
			return result;
		}

		public static implicit operator BoundingBoxStorage(BoundingBox3D component)
		{
			BoundingBoxStorage result = new BoundingBoxStorage();
			 result.FillFrom(component);
			return result;
		}

	}
}

