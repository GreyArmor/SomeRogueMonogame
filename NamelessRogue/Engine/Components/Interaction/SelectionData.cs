using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{

	internal class SelectionData : Component
	{
		public Point SelectionStart {get;set;}
		public Point SelectionEnd { get; set; }
		public SelectionState SelectionState { get; set; }
	}
}
