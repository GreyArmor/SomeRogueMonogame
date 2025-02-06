using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Generation.Editor;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class PickDialogOptionCommand : ICommand
    {
        public PickDialogOptionCommand(DialogOption option)
        {
            Option = option;
        }

        public DialogOption Option { get; }
    }

}
