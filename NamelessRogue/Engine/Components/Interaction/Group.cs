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
		public List<string> EntitiesInGroup { get; set; } = new List<string>();
		public bool FormationMaintained { get; set; } = true;
	}
}
