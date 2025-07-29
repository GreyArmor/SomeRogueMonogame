using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class NewGamePickWorldScreenSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();
        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while(namelessGame.Commander.DequeueCommand(out ExitContextCommand command))
            {
                namelessGame.ContextToSwitch = ContextFactory.GetMainMenuContext(namelessGame);
            }

            while (namelessGame.Commander.DequeueCommand(out StartNewGameInAWorldCommand command))
            {
                namelessGame.ContextToSwitch = ContextFactory.GetCharacterCreationScreenContext(namelessGame);
                namelessGame.Commander.EnqueueCommand(command);
                break;
            }
        }
    }
}

