using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class ActivateTargetedAbilityCommand : Abstraction.ICommand, IActivatedAbilityCommand
    {
        public ActivateTargetedAbilityCommand(Entity source, Entity ability)
        {
            Source = source;
            Ability = ability;
        }

        public Entity Source { get; }
        public Entity Ability { get; }
    }

    internal class ActivateSelfAbilityCommand : Abstraction.ICommand, IActivatedAbilityCommand
    {
        public ActivateSelfAbilityCommand(Entity source, Entity ability)
        {
            Source = source;
            Ability = ability;
        }

        public Entity Source { get; }
        public Entity Ability { get; }
    }

}
