using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
	internal class Group : Component
	{
		public Group(string testId) {
			TextId = testId;
		}
		public string TextId { get; set; }
		public List<IEntity> EntitiesInGroup { get; set; } = new List<IEntity>();
		public bool FormationMaintained { get; set; } = true;
		public IEntity FlagbearerId { get; internal set; }
	}
}
