using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
	internal class MuteUnmuteSoundCommand : ICommand
	{
		public MuteUnmuteSoundCommand(bool isMuted)
		{
			IsMuted = isMuted;
		}

		public bool IsMuted { get; }
	}
}
