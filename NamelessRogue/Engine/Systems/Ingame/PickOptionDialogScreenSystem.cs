using Microsoft.CodeAnalysis.Options;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Generation.Editor;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class PickOptionDialogScreenSystem : BaseSystem
    { 
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out StartItemPickUpDialogCommand command))
            {
                var currentOptions = new List<DialogPickOption>();

                currentOptions.Add(new DialogPickOption() { OptionData = null, Text = "Pick up everything" });
                foreach (var itemEntity in command.EntitiesToPickUp)
                {
                    var desc = itemEntity.GetComponentOfType<Description>();
                    currentOptions.Add(new DialogPickOption() { Text = $@"{desc.Name}", OptionData = itemEntity });
                }
                namelessGame.Commander.EnqueueCommand(new UpdatePickOptionDialogScreenCommand(currentOptions));
            }

            while (namelessGame.Commander.DequeueCommand(out UpdatePickOptionDialogScreenCommand command))
            {
                UIContainer.Instance.PickOptionDialogScreen.CurrentSelectedOptionIndex = 0;
                UIContainer.Instance.PickOptionDialogScreen.Options = command.CurrentOptions;
            }
        }
    }
}
