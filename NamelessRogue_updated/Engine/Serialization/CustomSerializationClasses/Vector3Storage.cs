﻿using FlatSharp.Attributes;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.Serialization.CustomSerializationClasses
{
	[FlatBufferTable]
	public class Vector3Storage : IStorage<Vector3>
	{
		[FlatBufferItem(0)] public float X { get; set; }
		[FlatBufferItem(1)] public float Y { get; set; }
		[FlatBufferItem(2)] public float Z { get; set; }
		public void FillFrom(Vector3 component)
		{
			X = component.X;
			Y = component.Y;
			Z = component.Z;
		}

		public void FillTo(Vector3 component)
		{
			component.X = X;
			component.Y = Y;
			component.Z = Z;
		}

		public static implicit operator Vector3(Vector3Storage thisType)
		{
			Vector3 result = new Vector3();
			thisType.FillTo(result);
			return result;
		}

		public static implicit operator Vector3Storage(Vector3 component)
		{
			Vector3Storage result = new Vector3Storage();
			result.FillFrom(result);
			return result;
		}

	}
}
