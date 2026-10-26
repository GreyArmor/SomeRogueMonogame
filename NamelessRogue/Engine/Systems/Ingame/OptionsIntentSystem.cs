using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Input;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems.Ingame
{
	public class OptionsIntentSystem : BaseSystem
	{
		public OptionsIntentSystem()
		{
			Signature = new HashSet<Type>();
		}
		public override HashSet<Type> Signature { get; }

		public override void Update(GameTime gameTime, NamelessGame namelessGame)
		{
			InputComponent inputComponent = namelessGame.InputEntity.GetComponentOfType<InputComponent>();
			if (inputComponent != null && !inputComponent.IsDelayed)
			{
				foreach (Intent intent in inputComponent.Intents)
				{
                    switch (intent.Intention)
                    { 
                    }
				}
				inputComponent.Intents.Clear();
			}
		}
	}

	public class CharactersStatsScreeIntentSystem : BaseSystem
	{
		public CharactersStatsScreeIntentSystem()
		{
			Signature = new HashSet<Type>();
		}
		public override HashSet<Type> Signature { get; }

		public override void Update(GameTime gameTime, NamelessGame namelessGame)
		{
			InputComponent inputComponent = namelessGame.InputEntity.GetComponentOfType<InputComponent>();
			if (inputComponent != null && !inputComponent.IsDelayed)
			{
				foreach (Intent intent in inputComponent.Intents)
				{
					switch (intent.Intention)
					{
					}
				}
				inputComponent.Intents.Clear();
			}
		}
	}

}
