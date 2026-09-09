using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Ingame
{
	internal class StreetlightsSystem : BaseSystem
	{	
		public override HashSet<Type> Signature { get; } = new HashSet<Type>() {  };

		public override void Update(GameTime gameTime, NamelessGame namelessGame)
		{	
			if(namelessGame.TurnUpdated)
			{
				if(namelessGame.CurrentGame.Turn % Constants.StreetLightChangeIntervalTurns == 0)
				{
					namelessGame.StreetLightsKeeper.GetComponentOfType<StreetlightsStatusKeeper>().Switch(); 
				}
			}
		}
	}
}
