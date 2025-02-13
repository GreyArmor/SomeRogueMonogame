using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.UI;
using System.Collections.Generic;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class UpdatePickOptionDialogScreenCommand : ICommand
    {
        public UpdatePickOptionDialogScreenCommand(List<DialogPickOption> currentOptions)
        {
            CurrentOptions = currentOptions;
        }

        public List<DialogPickOption> CurrentOptions { get; }
    }
}