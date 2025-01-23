using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class ActivateTargetedAbilityCommand : Abstraction.ICommand, IActivatedAbilityCommand
    {
        public ActivateTargetedAbilityCommand(IEntity source, IEntity ability)
        {
            Source = source;
            Ability = ability;
        }

        public IEntity Source { get; }
        public IEntity Ability { get; }
    }

    internal class ActivateSelfAbilityCommand : Abstraction.ICommand, IActivatedAbilityCommand
    {
        public ActivateSelfAbilityCommand(IEntity source, IEntity ability)
        {
            Source = source;
            Ability = ability;
        }

        public IEntity Source { get; }
        public IEntity Ability { get; }
    }

}
