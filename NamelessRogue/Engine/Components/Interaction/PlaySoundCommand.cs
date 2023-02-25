using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Sounds;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.Components.Interaction
{

	public class PlaySoundCommand : ICommand
	{
		public bool IsSong { get; set; }
		public bool OnLoop { get; set; }
		public Sound SoundToPlay
		{
			get; set;
		}
		public float Volume { get; set; }
		public PlaySoundCommand(Sound soundToPlay, float volume = 1, bool onLoop = false)
		{
			SoundToPlay = soundToPlay;
			Volume = volume;
			OnLoop = onLoop;
		} 
	}
}
