using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Input;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class PickOptionDialogIntentSystem : BaseSystem
    {
        public PickOptionDialogIntentSystem()
        {
            Signature = new HashSet<Type>();
        }
        public override HashSet<Type> Signature { get; }

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            InputComponent inputComponent = namelessGame.InputEntity.GetComponentOfType<InputComponent>();
            if (inputComponent != null && !inputComponent.IsDelayed)
            {
                foreach (Intent intent in inputComponent.Intents)
                {
                    switch (intent.Intention)
                    {
                        case IntentEnum.MoveUp:
                            {
                                UIContainer.Instance.PickOptionDialogScreen.CurrentSelectedOptionIndex--;
                                break;
                            }
                        case IntentEnum.MoveDown:
                            {
                                UIContainer.Instance.PickOptionDialogScreen.CurrentSelectedOptionIndex++;
                                break;
                            }
                        case IntentEnum.Interact:
                            {
                                var options = UIContainer.Instance.PickOptionDialogScreen.Options;
                                if (options.Count > 0)
                                {
                                    PickOptionDialogOptionCommand command = new PickOptionDialogOptionCommand(options[UIContainer.Instance.PickOptionDialogScreen.CurrentSelectedOptionIndex]);
                                    namelessGame.Commander.EnqueueCommand(command);
                                }
                                break;
                            }
                        case IntentEnum.QuickBarPress:
                            {
                                var parsed = int.TryParse(intent.PressedChar.ToString(), out int optionIndex);
                                if (!parsed)
                                {
                                    break;
                                }
                                var options = UIContainer.Instance.PickOptionDialogScreen.Options;
                                if (optionIndex >= 0 && optionIndex < options.Count)
                                {
                                    PickOptionDialogOptionCommand command = new PickOptionDialogOptionCommand(options[optionIndex]);
                                    namelessGame.Commander.EnqueueCommand(command);
                                }
                            }
                            break;
                        case IntentEnum.Enter:
                            goto case IntentEnum.Interact;
                        default:
                            break;
                    }
                }
                inputComponent.Intents.Clear();
            }
        }
    }
}
