using System;
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
                                var prevIndex = UiFactory.InventoryScreen.ItemBox.SelectedIndex.Value;
                                UiFactory.InventoryScreen.ItemBox.OnKeyDown(Keys.Down); /* += 1;*/

                                int nextIndex = UiFactory.InventoryScreen.ItemBox.SelectedIndex.Value;

                                bool move = false;
                                if (UiFactory.InventoryScreen.ItemBox.SelectedIndex == prevIndex)
                                {
                                    nextIndex = 0;
                                    move = true;
                                }

                                if (UiFactory.InventoryScreen.ItemBox.Items.Any())
                                {
                                    UiFactory.InventoryScreen.ItemBox.SelectedIndex = nextIndex;
                                    if (move)
                                    {
                                        UiFactory.InventoryScreen.ItemBox.OnKeyDown(Keys.Down);
                                        UiFactory.InventoryScreen.ItemBox.OnKeyDown(Keys.Up);
                                    }
                                }

                                break;
                            }
                            case IntentEnum.MoveUp:
                            {
                                var prevIndex = UiFactory.InventoryScreen.ItemBox.SelectedIndex.Value;
                                UiFactory.InventoryScreen.ItemBox.OnKeyDown(Keys.Up); /* -= 1;*/
                                int nextIndex = UiFactory.InventoryScreen.ItemBox.SelectedIndex.Value;
                                bool move = false;
                                if (UiFactory.InventoryScreen.ItemBox.SelectedIndex == prevIndex)
                                {
                                    nextIndex = UiFactory.InventoryScreen.ItemBox.Items.Count - 1;
                                    move = true;
                                }

                                if (UiFactory.InventoryScreen.ItemBox.Items.Any())
                                {
                                    UiFactory.InventoryScreen.ItemBox.SelectedIndex = nextIndex;
                                    if (move)
                                    {
                                        UiFactory.InventoryScreen.ItemBox.OnKeyDown(Keys.Up);
                                        UiFactory.InventoryScreen.ItemBox.OnKeyDown(Keys.Down);

                                    }
                                }

                                break;
                            }

                            case IntentEnum.ConetextualHoteyPressed:
                                //TODO add contextual hotkeys logic here
                                break;
                            case IntentEnum.Enter:
                            {
                                int selectedIndex = UiFactory.InventoryScreen.ItemBox.SelectedIndex.Value;
                                if (selectedIndex > 0)
                                {
                                    var equipmentEntity = (IEntity) UiFactory.InventoryScreen.ItemBox.SelectedItem.Tag;
                                    var itemsHolder = playerEntity.GetComponentOfType<ItemsHolder>();
                                    var equipment = playerEntity.GetComponentOfType<EquipmentSlots>();

                                    var equipmentItem = equipmentEntity.GetComponentOfType<Equipment>();
                                    equipment.Equip(equipmentItem);
                                    UiFactory.InventoryScreen.FillItems(namelessGame);
                                    //Dialog d = new Dialog();
                                    //d.ShowModal(new Point(0));

                                }
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
