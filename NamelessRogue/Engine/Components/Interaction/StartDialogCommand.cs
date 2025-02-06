using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Generation.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class StartDialogCommand : ICommand
    {
        private IEntity entityToTalkTo;

        public StartDialogCommand(IEntity entityToTalkTo)
        {
            this.EntityToTalkTo = entityToTalkTo;
        }

        public IEntity EntityToTalkTo { get => entityToTalkTo; set => entityToTalkTo = value; }
    }

    public class UpdateDialogScreenCommand : ICommand
    {
        public UpdateDialogScreenCommand(IEntity currentDialogEntity, DialogData currentDialogData)
        {
            CurrentDialogEntity = currentDialogEntity;
            CurrentDialogData = currentDialogData;
        }

        public IEntity CurrentDialogEntity { get; }
        public DialogData CurrentDialogData { get; }
    }

}
