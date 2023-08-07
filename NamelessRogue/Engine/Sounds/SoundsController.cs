using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Text;
using XnaSong = Microsoft.Xna.Framework.Media.Song;
namespace NamelessRogue.Engine.Sounds
{

	public static class SoundsHolder
	{
		public static Dictionary<string, SoundEffect> SoundDictionary { get; set; } = new Dictionary<string, SoundEffect>();
		public static Dictionary<string, XnaSong> SongDictionary { get; set; } = new Dictionary<string, XnaSong>();
	}
}
