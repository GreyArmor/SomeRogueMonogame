﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
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
                        switch (intent)
                        {
                            case Intent.MoveDown:
                            {
                                UiFactory.InventoryScreen.ItemBox.SelectedIndex += 1;

                                int nextIndex = 0;
                                if (UiFactory.InventoryScreen.ItemBox.SelectedIndex == null)
                                {
                                    nextIndex = 0;
                                }
                                else
                                {
                                    nextIndex = UiFactory.InventoryScreen.ItemBox.SelectedIndex.Value;
                                }

                                UiFactory.InventoryScreen.ItemBox.SelectedItem =
                                    UiFactory.InventoryScreen.ItemBox.Items[nextIndex];
                                UiFactory.InventoryScreen.ItemBox.ScrollToSelectedItem();
                                    break;
                            }
                            case Intent.MoveUp:
                            {
                                UiFactory.InventoryScreen.ItemBox.SelectedIndex -= 1;



                                int nextIndex = 0;
                                if (UiFactory.InventoryScreen.ItemBox.SelectedIndex == null)
                                {
                                    nextIndex = UiFactory.InventoryScreen.ItemBox.Items.Count - 1;
                                }
                                else
                                {
                                    nextIndex = UiFactory.InventoryScreen.ItemBox.SelectedIndex.Value;
                                }


                                UiFactory.InventoryScreen.ItemBox.SelectedItem =
                                    UiFactory.InventoryScreen.ItemBox.Items[nextIndex];
                                UiFactory.InventoryScreen.ItemBox.ScrollToSelectedItem();
                                break;
                            }

                            case Intent.Enter:
                            {

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
