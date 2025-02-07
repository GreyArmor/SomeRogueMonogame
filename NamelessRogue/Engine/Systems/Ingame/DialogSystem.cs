using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Generation.Editor;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class DialogSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();
        private DialogData currentDialogData;
        private IEntity currentDialogEntity;


        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out StartDialogCommand command))
            {
                var commandEtity = command.EntityToTalkTo;
                var dialogComponent = commandEtity.GetComponentOfType<DialogComponent>();
                if (dialogComponent != null)
                {
                    currentDialogEntity = commandEtity;
                    var dialogLibraryId = dialogComponent.DialogLibraryId;
                    var dialogData = DialogLibrary.DataById[dialogLibraryId];
                    currentDialogData = dialogData;
                    namelessGame.Commander.EnqueueCommand(new UpdateDialogScreenCommand(currentDialogEntity, currentDialogData));
                }
            }

            while (namelessGame.Commander.DequeueCommand(out PickDialogOptionCommand command))
            {
                var option = command.Option;

                switch (option.DialogOutcomeId)
                {
                    case DialogOutcomeId.None:
                        currentDialogData = option.DialogData;
                        namelessGame.Commander.EnqueueCommand(new UpdateDialogScreenCommand(currentDialogEntity, currentDialogData));
                        break;
                    case DialogOutcomeId.CancelDialog:
                        namelessGame.ContextToSwitch = ContextFactory.GetIngameContext(namelessGame);
                        break;
                    default:
                        namelessGame.ContextToSwitch = ContextFactory.GetIngameContext(namelessGame);
                        break;
                }
            }
        }
    }
}
