using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class ActivateAbilityCommand : Abstraction.ICommand
    {
        public ActivateAbilityCommand(IEntity source, IEntity ability)
        {
            Source = source;
            Ability = ability;
        }

        public IEntity Source { get; }
        public IEntity Ability { get; }
    }
}
