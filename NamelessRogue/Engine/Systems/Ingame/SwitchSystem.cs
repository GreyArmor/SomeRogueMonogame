using System;
using System.Collections.Generic;
using SharpDX;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.shell;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class SwitchSystem : BaseSystem
    {
        public SwitchSystem()
        {
            Signature = new HashSet<Type>();
        }
        public override HashSet<Type> Signature { get; }

        public override void Update(GameTime gameTime, NamelessGame game)
        {
            while (game.Commander.DequeueCommand(out ChangeSwitchStateCommand command))
            {
                command.getTarget().setSwitchActive(command.isActive());
            }
        }

	}
}
