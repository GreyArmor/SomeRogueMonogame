using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public enum IngameIntentSystemMode
    {
        PlayerMovement, FireWeapon, ActivatedAbility
    }
    internal class IngameIntentSystemModeSwitchCommand : ICommand
    {
        public IngameIntentSystemModeSwitchCommand(IngameIntentSystemMode mode)
        {
            Mode = mode;
        }

        public IngameIntentSystemMode Mode { get; }
    }
}
