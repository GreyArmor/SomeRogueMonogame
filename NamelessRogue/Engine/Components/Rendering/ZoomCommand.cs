using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.Components.Rendering
{
	public class ZoomCommand : ICommand
	{
		public ZoomCommand(bool zoomOut = true) {
			ZoomOut = zoomOut;
		}

		public bool ZoomOut { get; }
	}
}
