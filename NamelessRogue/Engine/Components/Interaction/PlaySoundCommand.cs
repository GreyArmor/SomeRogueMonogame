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
		public int LoopStart { get; }
		public int LoopEnd { get; }
		public string SoundToPlay
		{
			get; set;
		}
		public float Volume { get; set; }
		public PlaySoundCommand(string soundToPlay, bool isSong, float volume = 1, bool onLoop = false, int loopStart = 0, int loopEnd = 0)
		{
			SoundToPlay = soundToPlay;
			IsSong = isSong;
			Volume = volume;
			OnLoop = onLoop;
			LoopStart = loopStart;
			LoopEnd = loopEnd;
		} 
	}
}
