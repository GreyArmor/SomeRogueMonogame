﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
	internal class SelectedUnitsData : Component
	{
		public List<string> SelectedGroups { get; set; } = new List<string>();
	}
}
