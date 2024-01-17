using NamelessRogue.Engine.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
	internal class GroupsHolder : Component
	{
		public List<Entity> Groups { get; set; } = new List<Entity>(); 
	}
}
