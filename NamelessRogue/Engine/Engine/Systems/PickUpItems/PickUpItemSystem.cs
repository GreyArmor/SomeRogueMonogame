﻿using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Factories;
using NamelessRogue.Engine.Engine.Input;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Engine.UiScreens;

namespace NamelessRogue.Engine.Engine.Systems.PickUpItems
{
    public class PickUpItemSystem : BaseSystem
    {
        public PickUpItemSystem()
        {
            Signature = new HashSet<Type>();
            Signature.Add(typeof(InputComponent));
        }
        public override HashSet<Type> Signature { get; }

        public override void Update(long gameTime, NamelessGame namelessGame)
        {

            foreach (IEntity entity in RegisteredEntities)
            {
                if (entity.GetComponentOfType<UpdatePickupDialogCommand>()!=null)
                {
                    UiFactory.PickUpItemsScreen.FillItems(namelessGame);
                    entity.RemoveComponentOfType<UpdatePickupDialogCommand>();
                }

                InputComponent inputComponent = entity.GetComponentOfType<InputComponent>();
                if (inputComponent != null)
                {
                    var playerEntity = namelessGame.PlayerEntity;
                    foreach (Intent intent in inputComponent.Intents)
                    {
                        switch (intent.Intention)
                        {
                            case IntentEnum.MoveDown:
                            {
                                UiFactory.PickUpItemsScreen.ScrollSelectedTableDown();
                                break;
                            }
                            case IntentEnum.MoveUp:
                            {
                                UiFactory.PickUpItemsScreen.ScrollSelectedTableUp();
                                break;
                            }
                            case IntentEnum.ConetextualHotkeyPressed:
                                var selectedItem =
                                    UiFactory.PickUpItemsScreen.SelectedTable.Items.FirstOrDefault(x =>
                                        x.Hotkey == intent.PressedChar);

                                if (selectedItem != null)
                                {
                                    UiFactory.PickUpItemsScreen.SelectedTable.OnItemClick.Invoke(selectedItem);
                                }

                                break;
                            case IntentEnum.Enter:
                            {
                                UiFactory.PickUpItemsScreen.SelectedTable.OnItemClick.Invoke(UiFactory.PickUpItemsScreen
                                    .SelectedTable.SelectedItem);
                            }
                                break;
                            default:
                                break;
                        }
                    }

                    inputComponent.Intents.Clear();
                }
            }



            foreach (var action in UiFactory.PickUpItemsScreen.Actions)
            {
                switch (action)
                {
                    case PickUpItemsScreenAction.ReturnToGame:
                        namelessGame.ContextToSwitch = ContextFactory.GetIngameContext(namelessGame);
                        break;
                    default:
                        break;
                }
            }

            UiFactory.PickUpItemsScreen.Actions.Clear();


        }
    }
}
