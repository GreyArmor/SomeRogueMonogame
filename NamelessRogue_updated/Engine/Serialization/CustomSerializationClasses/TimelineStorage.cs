using FlatSharp.Attributes;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.Serialization.CustomSerializationClasses
{
	[FlatBufferTable]
	public class TimelineStorage : IStorage<TimeLine>
	{
		[FlatBufferItem(0)] public string Id { get; set; }
		[FlatBufferItem(1)] public string ParentEntityId { get; set; }
		[FlatBufferItem(2)] public TimelineLayerStorage CurrentTimelineLayer { get; set; }
		public void FillFrom(TimeLine component)
		{
			Id = component.Id.ToString();
			ParentEntityId = component.ParentEntityId.ToString();
			CurrentTimelineLayer = component.CurrentTimelineLayer;
		}

		public void FillTo(TimeLine component)
		{
			component.Id = new Guid(Id);
			component.ParentEntityId = new Guid(ParentEntityId);
			component.CurrentTimelineLayer = CurrentTimelineLayer;
		}
	}
}
