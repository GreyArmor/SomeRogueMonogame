using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FbxSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Input;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems.Inventory
{
    public class InventoryScreenSystem : BaseSystem
    {
        public InventoryScreenSystem()
        {
            Signature = new HashSet<Type>();
            Signature.Add(typeof(InputComponent));
        }

        public override bool IsEntityMatchesSignature(IEntity entity)
        {
            var entityComponentTypes = new HashSet<Type>(entity.GetAllComponents().Select(x => x.GetType()));

            foreach (var type in Signature)
            {
                if (entityComponentTypes.Contains(type))
                {
                    return true;
                }
            }
            return false;
        }

        public override HashSet<Type> Signature { get; }
        public bool InventoryNeedsUpdate { get; private set; }

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            if (InventoryNeedsUpdate)
            {
              //  UIController.InventoryScreen.FillItems(namelessGame);
                InventoryNeedsUpdate = false;
            }

            //foreach (var action in UIController.Instance.InventoryScreen.Actions)
            //{
            //    action.Invoke(this, namelessGame);
            //}

          //  UIController.Instance.InventoryScreen.Actions.Clear();

            foreach (IEntity entity in RegisteredEntities)
            {
                InputComponent inputComponent = entity.GetComponentOfType<InputComponent>();
                if (inputComponent != null)
                {
                    var playerEntity = namelessGame.PlayerEntity;
                    foreach (Intent intent in inputComponent.Intents)
                    {
                       //// if (UIController.Instance.InventoryScreen.AmountDialog != null)
                       // {
                       //     switch (intent.Intention)
                       //     {
                       //         case IntentEnum.Enter:
                       //             {
                       //                // UIController.Instance.InventoryScreen.AmountDialog.ButtonOk.DoClick();
                       //             }
                       //             break;
                       //         case IntentEnum.Escape:
                       //             {
                       //                 //UIController.Instance.InventoryScreen.AmountDialog.ButtonCancel.DoClick();
                       //             }
                       //             break;
                       //         default:
                       //             break;
                       //     }
                       // }
                       // else
                        {

                            var pos = UIController.Instance.InventoryScreen.SelectedCell;
                            switch (intent.Intention)
                            {
                                case IntentEnum.MoveDown:
                                    {
                                       
                                        UIController.Instance.InventoryScreen.SelectedCell = new System.Drawing.Point(pos.X, pos.Y+1);
                                        break;
                                    }
                                case IntentEnum.MoveUp:
                                    {
                                        UIController.Instance.InventoryScreen.SelectedCell = new System.Drawing.Point(pos.X, pos.Y - 1);
                                        break;
                                    }
                                case IntentEnum.MoveLeft:
                                    {
                                        UIController.Instance.InventoryScreen.SelectedCell = new System.Drawing.Point(pos.X - 1 , pos.Y);
                                        break;
                                    }

                                case IntentEnum.MoveRight:
                                    {
                                        UIController.Instance.InventoryScreen.SelectedCell = new System.Drawing.Point(pos.X + 1, pos.Y);
                                        break;
                                    }
                                case IntentEnum.ConetextualHotkeyPressed:
                                    //var selectedItem =
                                    //    UIController.Instance.InventoryScreen.SelectedTable.Items.FirstOrDefault(x =>
                                    //        x.Hotkey == intent.PressedChar);

                                    //if (selectedItem != null)
                                    //{
                                    //    UIController.Instance.SelectedTable.OnItemClick.Invoke(selectedItem);
                                    //}
                                    //else if (!UIController.Instance.DialogOpened)
                                    //{
                                    //    var selectedInItems =
                                    //        UIController.Instance.ItemBox.Items.FirstOrDefault(x =>
                                    //            x.Hotkey == intent.PressedChar);
                                    //    var selectedInEquipment =
                                    //        UIController.Instance.InventoryScreen.EquipmentBox.Items.FirstOrDefault(x =>
                                    //            x.Hotkey == intent.PressedChar);

                                    //    if (selectedInItems != null)
                                    //    {
                                    //        UIController.Instance.InventoryScreen.ItemBox.OnItemClick.Invoke(selectedInItems);
                                    //    }

                                    //    if (selectedInEquipment != null)
                                    //    {
                                    //        UIController.Instance.InventoryScreen.EquipmentBox.OnItemClick.Invoke(
                                    //            selectedInEquipment);
                                    //    }
                                    //}

                                    break;
                                case IntentEnum.Enter:
                                    {
                                    //    UIController.InventoryScreen.SelectedTable.OnItemClick.Invoke(UIController.InventoryScreen
                                    //        .SelectedTable.SelectedItem);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    inputComponent.Intents.Clear();
                }
            }


        }

        internal void BackToGame(NamelessGame game)
        {
            game.ContextToSwitch = ContextFactory.GetIngameContext(game);
        }

        internal void ScheduleUpdate()
        {
            InventoryNeedsUpdate = true;
        }
    }
}