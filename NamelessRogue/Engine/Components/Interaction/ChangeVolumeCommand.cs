using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
	internal class ChangeVolumeCommand : ICommand
	{
		public ChangeVolumeCommand(float volume)
		{
			Volume = volume;
		}

		public float Volume { get; }
	}
}
