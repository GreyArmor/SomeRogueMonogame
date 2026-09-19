using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Components.Interaction
{
	internal class Group : Component
	{
		public Group(string testId) {
			TextId = testId;
		}
		public string TextId { get; set; }
		public List<Entity> EntitiesInGroup { get; set; } = new List<Entity>();
		public bool FormationMaintained { get; set; } = true;
		public Entity FlagbearerId { get; internal set; }
	}
}
