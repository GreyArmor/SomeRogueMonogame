using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Input;
using NamelessRogue.shell;
using System.Collections.Generic;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class OptionsPopupSubprocessor : IngameIntentSystemSubProcessor
    {
        public List<IEntity> OptionItems { get; set; } = new List<IEntity>();

        public void Process(NamelessGame namelessGame, IngameIntentSystem system, Intent intent)
        {
            var playerEntity = namelessGame.PlayerEntity;
            switch (intent.Intention)
            {
                case IntentEnum.MoveUp:
                    {
                        namelessGame.CurrentContext.ContextScreen.CurrentOptionsItem--;
                    }
                    break;
                case IntentEnum.MoveDown:
                    {
                        namelessGame.CurrentContext.ContextScreen.CurrentOptionsItem++;
                    }
                    break;
                case IntentEnum.ZoomIn:
                    var zoomCommand = new ZoomCommand(false);
                    namelessGame.Commander.EnqueueCommand(zoomCommand);
                    break;
                case IntentEnum.ZoomOut:
                    var zoomOutCommand = new ZoomCommand();
                    namelessGame.Commander.EnqueueCommand(zoomOutCommand);
                    break;
                case IntentEnum.MouseChanged:
                    break;
                case IntentEnum.QuickBarPress:
                    system.SingleKeyPressIntents.Add(IntentEnum.QuickBarPress);
                    var parsed = int.TryParse(intent.PressedChar.ToString(), out int abilityIndex);
                    if(parsed && abilityIndex > 0 && abilityIndex < OptionItems.Count )
                    {
                        var chosenOptionEntity = OptionItems[abilityIndex];
                        namelessGame.Commander.EnqueueCommand(new PickUpItemCommand(new List<IEntity>() { chosenOptionEntity }, namelessGame.PlayerEntity.GetComponentOfType<ItemsHolder>()));
                        var switchModeCommand = new IngameIntentSystemModeSwitchCommand(IngameIntentSystemMode.PlayerMovement);
                        namelessGame.Commander.EnqueueCommand(switchModeCommand);
                        namelessGame.CurrentContext.ContextScreen.CloseOptionsPopUp();

                    }
                    break;
                case IntentEnum.Interact:
                    {
                        var chosenOptionEntity = OptionItems[namelessGame.CurrentContext.ContextScreen.CurrentOptionsItem];
                        namelessGame.Commander.EnqueueCommand(new PickUpItemCommand(new List<IEntity>() { chosenOptionEntity }, namelessGame.PlayerEntity.GetComponentOfType<ItemsHolder>()));

                        var switchModeCommand = new IngameIntentSystemModeSwitchCommand(IngameIntentSystemMode.PlayerMovement);
                        namelessGame.Commander.EnqueueCommand(switchModeCommand);
                        namelessGame.CurrentContext.ContextScreen.CloseOptionsPopUp();
                    }
                    break;
                case IntentEnum.Escape:
                    {
                        var switchModeCommand = new IngameIntentSystemModeSwitchCommand(IngameIntentSystemMode.PlayerMovement);
                        namelessGame.Commander.EnqueueCommand(switchModeCommand);

                        namelessGame.CurrentContext.ContextScreen.CloseOptionsPopUp();

                    }
                    break;
                case IntentEnum.SwitchTarget:
                    {
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
