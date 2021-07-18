
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatSharp.Attributes;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using NamelessRogue.Engine.Serialization.AutogeneratedSerializationClasses;

namespace NamelessRogue.Engine.Serialization.CustomSerializationClasses
{
	[FlatBufferTable]
	public class CivilizationStorage : IStorage<Generation.World.Civilization>
	{
		[FlatBufferItem(0)] public string Id { get; set; }
		[FlatBufferItem(1)] public string ParentEntityId { get; set; }
		[FlatBufferItem(2)] public string Name { get; set; }

		[FlatBufferItem(3)] public Dictionary<CivilizationStorage, PoliticalRelationStorage> Relations { get; set; }

		public void FillFrom(Generation.World.Civilization component)
		{

			Name = component.Name;

			Relations = new Dictionary<CivilizationStorage, PoliticalRelationStorage>();

			foreach (var pair in component.Relations)
			{
				Relations.Add(pair.Key, pair.Value);
			}

		}

		public void FillTo(Generation.World.Civilization component)
		{

			component.Name = Name;

			component.Relations = new Dictionary<Generation.World.Civilization, Generation.World.Diplomacy.PoliticalRelation>();
			
			foreach (var pair in Relations)
			{
				component.Relations.Add(pair.Key, pair.Value);
			}
		}

		public static implicit operator Generation.World.Civilization(CivilizationStorage thisType)
		{
			Generation.World.Civilization result = new Generation.World.Civilization();
			thisType.FillTo(result);
			return result;
		}

		public static implicit operator CivilizationStorage(Generation.World.Civilization component)
		{
			CivilizationStorage result = new CivilizationStorage();
			result.FillFrom(result);
			return result;
		}

	}

}