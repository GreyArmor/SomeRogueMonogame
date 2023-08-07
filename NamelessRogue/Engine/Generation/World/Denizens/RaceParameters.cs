using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Generation.World.Denizens
{
	internal class RaceParameters
	{
		public string Id {get; set;}	
		public string Name {get; set;}
		public int HealthAverage { get; set; }
		public int StrengthAverage { get; set; }
		public int HealthVariation { get; set; }
		public int StrengthVariation { get; set; }

		public List<Tuple<int, BehaviourTag>> Behaviors { get; set; }

		public RaceParameters() { }
	}



}
