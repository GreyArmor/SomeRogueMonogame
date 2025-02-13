using NamelessRogue.Engine.Abstraction;
using System.Collections.Generic;
using System.Windows.Input;
using ICommand = NamelessRogue.Engine.Abstraction.ICommand;

namespace NamelessRogue.Engine.UI
{
    internal class PickOptionDialogOptionCommand : ICommand
    {
        private List<DialogPickOption> dialogPickOptions = new List<DialogPickOption>();

        public PickOptionDialogOptionCommand(params DialogPickOption[] dialogPickOptions)
        {
            foreach (DialogPickOption option in dialogPickOptions)
            {
                this.dialogPickOptions.Add(option);
            }
        }

        public List<DialogPickOption> DialogPickOptions { get => dialogPickOptions; set => dialogPickOptions = value; }
    }
}