using System;
using System.Collections.Generic;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems.Ingame
{
    public class SwitchSystem : BaseSystem
    {
        public SwitchSystem()
        {
            Signature = new HashSet<Type>();
            Signature.Add(typeof(ChangeSwitchStateCommand));
        }
        public override HashSet<Type> Signature { get; }

        public override void Update(long gameTime, NamelessGame namelessGame)
        {
            foreach (IEntity entity in RegisteredEntities)
            {
                ChangeSwitchStateCommand command = entity.GetComponentOfType<ChangeSwitchStateCommand>();
                command.getTarget().setSwitchActive(command.isActive());
                entity.RemoveComponentOfType<ChangeSwitchStateCommand>();
            }
        }
    }
}
