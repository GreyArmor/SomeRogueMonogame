using FlatSharp.Attributes;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.Serialization.CustomSerializationClasses
{
	[FlatSharp.Attributes.FlatBufferTable]
	public class EntityStorage : IStorage<Entity>, IStorage<IEntity>
	{
		[FlatBufferItem(0)] Guid Id { get; set; }
		public void FillFrom(IEntity component)
		{
			Id = component.Id;
		}

		public void FillFrom(Entity component)
		{
			Id = component.Id;
		}

		public void FillTo(IEntity component)
		{
			component.Id = Id;
		}

		public void FillTo(Entity component)
		{
			component.Id = Id;
		}

		public static implicit operator Entity(EntityStorage thisType)
		{
			Entity result = new Entity();
			thisType.FillTo(result);
			return result;
		}

		public static implicit operator EntityStorage(Entity component)
		{
			EntityStorage result = new EntityStorage();
			result.FillFrom(result);
			return result;
		}

	}
}
