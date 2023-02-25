using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Sounds;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.Systems
{
	//TODO implement music looping
	public class SoundPlaySystem : BaseSystem
	{
		public override HashSet<Type> Signature { get; } = new HashSet<Type>();
		public List<SoundEffectInstance> SoundInstances { get; set; } = new List<SoundEffectInstance>();
		public override void Update(GameTime gameTime, NamelessGame namelessGame)
		{
			while (namelessGame.Commander.DequeueCommand(out PlaySoundCommand command))
			{
			
					SoundsHolder.SoundDictionary[command.SoundToPlay].Play(command.Volume, 0, 0);
			}
		}
	}
}
