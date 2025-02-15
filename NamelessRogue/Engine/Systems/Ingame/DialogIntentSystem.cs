using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Input;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class DialogIntentSystem : BaseSystem
    {
        public DialogIntentSystem()
        {
            Signature = new HashSet<Type>();
        }
        public override HashSet<Type> Signature { get; }

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            InputComponent inputComponent = namelessGame.PlayerEntity.GetComponentOfType<InputComponent>();
            if (inputComponent != null && !inputComponent.IsDelayed)
            {
                foreach (Intent intent in inputComponent.Intents)
                {
                    switch (intent.Intention)
                    {
                        case IntentEnum.MoveUp:
                            {
                                UIContainer.Instance.DialogScreen.CurrentSelectedOptionIndex--;
                                break;
                            }
                        case IntentEnum.MoveDown:
                            {
                                UIContainer.Instance.DialogScreen.CurrentSelectedOptionIndex++;
                                break;
                            }
                        case IntentEnum.Interact:
                            {
                                var options = UIContainer.Instance.DialogScreen.CurrentDialogData.Options;
                                PickDialogOptionCommand command = new PickDialogOptionCommand(options[UIContainer.Instance.DialogScreen.CurrentSelectedOptionIndex]);
                                namelessGame.Commander.EnqueueCommand(command);
                                break;
                            }
                        case IntentEnum.QuickBarPress:
                            {
                                var parsed = int.TryParse(intent.PressedChar.ToString(), out int optionIndex);
                                if (!parsed)
                                {
                                    break;
                                }
                                var options = UIContainer.Instance.DialogScreen.CurrentDialogData.Options;
                                if (optionIndex >= 0 && optionIndex < options.Length)
                                {
                                    PickDialogOptionCommand command = new PickDialogOptionCommand(options[optionIndex]);
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
