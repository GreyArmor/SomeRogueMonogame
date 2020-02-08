﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Engine.Factories;
using NamelessRogue.Engine.Engine.Input;
using NamelessRogue.Engine.Engine.UiScreens;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems.Inventory
{
    public class InventoryScreenSystem : ISystem
    {
        public void Update(long gameTime, NamelessGame namelessGame)
        {

            foreach (IEntity entity in namelessGame.GetEntities())
            {
                InputComponent inputComponent = entity.GetComponentOfType<InputComponent>();
                if (inputComponent != null)
                {
                    var playerEntity = namelessGame.GetEntitiesByComponentClass<Player>().First();
                    foreach (Intent intent in inputComponent.Intents)
                    {
                        switch (intent.Intention)
                        {
                            case IntentEnum.MoveDown:
                            {
                                UiFactory.InventoryScreen.ScrollSelectedTableDown();
                                break;
                            }
                            case IntentEnum.MoveUp:
                            {
                                UiFactory.InventoryScreen.ScrollSelectedTableUp();
                                break;
                            }
                            case IntentEnum.MoveLeft:
                            {
                                UiFactory.InventoryScreen.SwitchSelectedTable();
                                break;
                            }

                            case IntentEnum.MoveRight:
                            {
                                UiFactory.InventoryScreen.SwitchSelectedTable();
                                break;
                            }
                            case IntentEnum.ConetextualHotkeyPressed:
                                var selectedItem =
                                    UiFactory.InventoryScreen.SelectedTable.Items.FirstOrDefault(x =>
                                        x.Hotkey == intent.PressedChar);

                                if (selectedItem != null)
                                {
                                    UiFactory.InventoryScreen.SelectedTable.OnItemClick.Invoke(selectedItem);
                                }
                                else
                                {
                                    var selectedInItems =
                                        UiFactory.InventoryScreen.ItemBox.Items.FirstOrDefault(x =>
                                            x.Hotkey == intent.PressedChar);
                                    var selectedInEquipment = UiFactory.InventoryScreen.EquipmentBox.Items.FirstOrDefault(x =>
                                        x.Hotkey == intent.PressedChar);

                                    if (selectedInItems != null)
                                    {
                                        UiFactory.InventoryScreen.ItemBox.OnItemClick.Invoke(selectedInItems);
                                    }
                                    if (selectedInEquipment != null)
                                    {
                                        UiFactory.InventoryScreen.EquipmentBox.OnItemClick.Invoke(selectedInEquipment);
                                    }
                                }


                                break;
                            case IntentEnum.Enter:
                            {
                               UiFactory.InventoryScreen.SelectedTable.OnItemClick.Invoke(UiFactory.InventoryScreen.SelectedTable.SelectedItem);
                            }
                                break;
                            default:
                                break;
                        }
                    }

                    inputComponent.Intents.Clear();
                }
            }



            foreach (var action in UiFactory.InventoryScreen.Actions)
            {
                switch (action)
                {
                    case InventoryScreenAction.ReturnToGame:
                        namelessGame.ContextToSwitch = ContextFactory.GetIngameContext(namelessGame);
                        break;
                    default:
                        break;
                }
            }

            UiFactory.InventoryScreen.Actions.Clear();


        }
    }
}