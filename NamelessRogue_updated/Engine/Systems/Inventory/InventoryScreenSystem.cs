using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Input;
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

            /*
            if (InventoryNeedsUpdate)
            {
                UIController.InventoryScreen.FillItems(namelessGame);
                InventoryNeedsUpdate = false;
            }

            foreach (var action in UIController.InventoryScreen.Actions)
            {
                action.Invoke(this, namelessGame);
            }

            UIController.InventoryScreen.Actions.Clear();

            foreach (IEntity entity in RegisteredEntities)
            {
                InputComponent inputComponent = entity.GetComponentOfType<InputComponent>();
                if (inputComponent != null)
                {
                    var playerEntity = namelessGame.PlayerEntity;
                    foreach (Intent intent in inputComponent.Intents)
                    {
                        if (UIController.InventoryScreen.AmountDialog != null)
                        {
                            switch (intent.Intention)
                            {
                                case IntentEnum.Enter:
                                {
                                    UIController.InventoryScreen.AmountDialog.ButtonOk.DoClick();
                                }
                                    break;
                                case IntentEnum.Escape:
                                {
                                    UIController.InventoryScreen.AmountDialog.ButtonCancel.DoClick();
                                }
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {


                            switch (intent.Intention)
                            {
                                case IntentEnum.MoveDown:
                                {
                                    UIController.InventoryScreen.ScrollSelectedTableDown();
                                    break;
                                }
                                case IntentEnum.MoveUp:
                                {
                                    UIController.InventoryScreen.ScrollSelectedTableUp();
                                    break;
                                }
                                case IntentEnum.MoveLeft:
                                {
                                    UIController.InventoryScreen.SwitchSelectedTable();
                                    break;
                                }

                                case IntentEnum.MoveRight:
                                {
                                    UIController.InventoryScreen.SwitchSelectedTable();
                                    break;
                                }
                                case IntentEnum.ConetextualHotkeyPressed:
                                    var selectedItem =
                                        UIController.InventoryScreen.SelectedTable.Items.FirstOrDefault(x =>
                                            x.Hotkey == intent.PressedChar);

                                    if (selectedItem != null)
                                    {
                                        UIController.InventoryScreen.SelectedTable.OnItemClick.Invoke(selectedItem);
                                    }
                                    else if (!UIController.InventoryScreen.DialogOpened)
                                    {
                                        var selectedInItems =
                                            UIController.InventoryScreen.ItemBox.Items.FirstOrDefault(x =>
                                                x.Hotkey == intent.PressedChar);
                                        var selectedInEquipment =
                                            UIController.InventoryScreen.EquipmentBox.Items.FirstOrDefault(x =>
                                                x.Hotkey == intent.PressedChar);

                                        if (selectedInItems != null)
                                        {
                                            UIController.InventoryScreen.ItemBox.OnItemClick.Invoke(selectedInItems);
                                        }

                                        if (selectedInEquipment != null)
                                        {
                                            UIController.InventoryScreen.EquipmentBox.OnItemClick.Invoke(
                                                selectedInEquipment);
                                        }
                                    }

                                    break;
                                case IntentEnum.Enter:
                                {
                                    UIController.InventoryScreen.SelectedTable.OnItemClick.Invoke(UIController.InventoryScreen
                                        .SelectedTable.SelectedItem);
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
            */

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