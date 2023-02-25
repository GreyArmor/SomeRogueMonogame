using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Text;
using XnaSong = Microsoft.Xna.Framework.Media.Song;
namespace NamelessRogue.Engine.Sounds
{
	public enum Sound
	{
		ButtonClick
	}

	public enum Song
	{
		MainMenu
	}

	public static class SoundsHolder
	{
		public static Dictionary<Sound, SoundEffect> SoundDictionary { get; set; } = new Dictionary<Sound, SoundEffect>();
		public static Dictionary<Song, XnaSong> SongDictionary { get; set; } = new Dictionary<Song, XnaSong>();
	}
}
