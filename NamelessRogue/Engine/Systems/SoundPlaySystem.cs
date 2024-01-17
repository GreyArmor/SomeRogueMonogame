using Veldrid;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Sounds;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Text;
using NamelessRogue.Engine.Infrastructure;
namespace NamelessRogue.Engine.Systems
{
	//TODO implement music looping
	public class SoundPlaySystem : BaseSystem
	{
		public override HashSet<Type> Signature { get; } = new HashSet<Type>();
	//	public List<SoundEffectInstance> SoundInstances { get; set; } = new List<SoundEffectInstance>();
	//	public Song CurrentSong {get;set; }
		bool songsFadeIn = true;
		float fadespeed = 0.0001f;
		float maxVolume = 1f;
		bool isFadingIn = false;
		private void _fadeIn(double delta)
		{
			//float volume = MediaPlayer.Volume;
			//volume += (float)(delta * fadespeed);
			//if (volume > maxVolume)
			//{
			//	volume = maxVolume;
			//	isFadingIn = false;
			//}
			//MediaPlayer.Volume = volume;
		}

		public override void Update(GameTime gameTime, NamelessGame game)
		{
			//return;

			//if (isFadingIn)
			//{
			//	_fadeIn(GameTime.ElapsedGameTime.Milliseconds);
			//}


			//while (game.Commander.DequeueCommand(out PlaySoundCommand command))
			//{
			//	if (command.IsSong)
			//	{
			//		SoundsHolder.SongDictionary.TryGetValue(command.SoundToPlay, out var sound);
			//		MediaPlayer.Stop();
					
			//		MediaPlayer.Play(sound);
			//		MediaPlayer.IsRepeating = true;

			//		if (songsFadeIn)
			//		{
			//			MediaPlayer.Volume = 0;
			//			maxVolume = command.Volume;
			//			isFadingIn = true;
			//		}
			//		else {
			//			MediaPlayer.Volume = command.Volume;
			//		}
					
			//	}
			//	else
			//	{
			//		if (!MediaPlayer.IsMuted)
			//		{
			//			SoundsHolder.SoundDictionary.TryGetValue(command.SoundToPlay, out var sound);
			//			sound?.Play(command.Volume, 0, 0);
			//		}
			//	}
			//}

			//while (game.Commander.DequeueCommand(out MuteUnmuteSoundCommand command))
			//{
			//	MediaPlayer.IsMuted = command.IsMuted;
			//}

			//while (game.Commander.DequeueCommand(out ChangeVolumeCommand command))
			//{
			//	MediaPlayer.Volume = command.Volume;
			//}
		}
	}
}
