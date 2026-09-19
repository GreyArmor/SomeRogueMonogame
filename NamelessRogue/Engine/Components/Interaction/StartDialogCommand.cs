using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Generation.Editor;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class StartDialogCommand : ICommand
    {
        private Entity entityToTalkTo;

        public StartDialogCommand(Entity entityToTalkTo)
        {
            this.EntityToTalkTo = entityToTalkTo;
        }

        public Entity EntityToTalkTo { get => entityToTalkTo; set => entityToTalkTo = value; }
    }

    public class UpdateDialogScreenCommand : ICommand
    {
        public UpdateDialogScreenCommand(Entity currentDialogEntity, DialogData currentDialogData)
        {
            CurrentDialogEntity = currentDialogEntity;
            CurrentDialogData = currentDialogData;
        }

        public Entity CurrentDialogEntity { get; }
        public DialogData CurrentDialogData { get; }
    }

}
