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
        PlayerMovement, FireWeapon, QuickBarAiming,
        OptionsPopup
    }
    internal class IngameIntentSystemModeSwitchCommand : ICommand
    {
        //TODO: could be way better, refactor
        public IngameIntentSystemModeSwitchCommand(IngameIntentSystemMode mode, object commandData = null)
        {
            Mode = mode;
            CommandData = commandData;
        }

        public IngameIntentSystemMode Mode { get; }
        public object CommandData { get; }
    }
}
