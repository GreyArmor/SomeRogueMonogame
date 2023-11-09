﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
	internal class GroupTag : Component
	{
		public string GroupId { get; set; }
		public bool IsInFormation { get; set; } = true;
	}
}