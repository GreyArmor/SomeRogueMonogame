using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NamelessRogue.Engine.Sounds
{
	[XmlRoot]
	public struct Track { 
		[XmlElement]
		public string Name;
		[XmlElement]
		public string File;
		[XmlElement]
		public string Artist;
		[XmlElement]
		public string ThemeId;
		[XmlElement]
		public bool Loopable;
		[XmlElement]
		public int LoopStartSecond;
		[XmlElement]
		public int LoopEndSecond;

	}
	[XmlRoot]
	public class MusicPack
	{
		[XmlArray]
		public List<Track> Tracks;
	}
}
