using FlatSharp.Attributes;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.Serialization.CustomSerializationClasses
{
	[FlatBufferTable]
	public class TimelineStorage : IStorage<WorldTemplate>
	{
		[FlatBufferItem(0)] public string Id { get; set; }
		[FlatBufferItem(1)] public string ParentEntityId { get; set; }
		public void FillFrom(WorldTemplate component)
		{
			Id = component.Id.ToString();
			ParentEntityId = component.ParentEntityId.ToString();

		}

		public void FillTo(WorldTemplate component)
		{
			component.Id = new Guid(Id);
			component.ParentEntityId = new Guid(ParentEntityId);
		}
	}
}
