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
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Input;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;
using SharpDX.DirectWrite;
using Point = System.Drawing.Point;

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

                            //spaghetti-o

                            Point position = default(Point);
                            switch (intent.Intention)
                            {
                                case IntentEnum.MoveUp:
                                case IntentEnum.MoveDown:
                                case IntentEnum.MoveLeft:
                                case IntentEnum.MoveRight:
                                case IntentEnum.MoveTopLeft:
                                case IntentEnum.MoveTopRight:
                                case IntentEnum.MoveBottomLeft:
                                case IntentEnum.MoveBottomRight:
                                case IntentEnum.MoveAscent:
                                case IntentEnum.MoveDescent:
                                    {
                                        switch (UIContainer.Instance.InventoryScreen.CursorMode)
                                        {

                                            case InventoryScreenCursorMode.PageSwitch:
                                                position = UIContainer.Instance.InventoryScreen.PageSwitchModel.SelectedCell;
                                                break;
                                            case InventoryScreenCursorMode.Items:
                                                position = UIContainer.Instance.InventoryScreen.GridModel.SelectedCell;
                                                break;
                                            case InventoryScreenCursorMode.Equipment:
                                                position = UIContainer.Instance.InventoryScreen.EquipmentVisualModel.SelectedCell;
                                                break;
                                            case InventoryScreenCursorMode.ItemsFilter:
                                                position = UIContainer.Instance.InventoryScreen.FiltersVisualModel.SelectedCell;
                                                break;
                                        }

                                        int newX =
                                            intent.Intention == IntentEnum.MoveLeft || intent.Intention == IntentEnum.MoveBottomLeft ||
                                            intent.Intention == IntentEnum.MoveTopLeft ? position.X - 1 :
                                            intent.Intention == IntentEnum.MoveRight || intent.Intention == IntentEnum.MoveBottomRight ||
                                            intent.Intention == IntentEnum.MoveTopRight ? position.X + 1 :
                                            position.X;

                                        int newY =
                                            intent.Intention == IntentEnum.MoveDown || intent.Intention == IntentEnum.MoveBottomLeft ||
                                            intent.Intention == IntentEnum.MoveBottomRight ? position.Y + 1 :
                                            intent.Intention == IntentEnum.MoveUp || intent.Intention == IntentEnum.MoveTopLeft ||
                                            intent.Intention == IntentEnum.MoveTopRight ? position.Y - 1 :
                                            position.Y;

                                        switch (UIContainer.Instance.InventoryScreen.CursorMode)
                                        {
                                            case InventoryScreenCursorMode.PageSwitch:
                                                if (newY < 0)
                                                {
                                                    UIContainer.Instance.InventoryScreen.CursorMode = InventoryScreenCursorMode.ItemsFilter;
                                                    UIContainer.Instance.InventoryScreen.FiltersVisualModel.SelectedCell = new Point(0, 0);
                                                }
                                                else if (newX < 0 || newX > 1)
                                                {
                                                }
                                                else if (newY > 0)
                                                {
                                                    UIContainer.Instance.InventoryScreen.CursorMode = InventoryScreenCursorMode.Items;
                                                    UIContainer.Instance.InventoryScreen.GridModel.SelectedCell = new Point(0, 0);
                                                }
                                                else
                                                {
                                                    UIContainer.Instance.InventoryScreen.PageSwitchModel.SelectedCell = new Point(newX, newY);
                                                }
                                                break;
                                            case InventoryScreenCursorMode.Items:

                                                if (newY < 0)
                                                {
                                                    UIContainer.Instance.InventoryScreen.CursorMode = InventoryScreenCursorMode.PageSwitch;
                                                    UIContainer.Instance.InventoryScreen.PageSwitchModel.SelectedCell = new Point(0, 0);
                                                }
                                                if (newX < 0)
                                                {
                                                    UIContainer.Instance.InventoryScreen.CursorMode = InventoryScreenCursorMode.Equipment;
                                                    UIContainer.Instance.InventoryScreen.EquipmentVisualModel.SelectedCell = new Point(2, 1);
                                                }
                                                if (newY >= 0 && newX >= 0)
                                                {
                                                    UIContainer.Instance.InventoryScreen.GridModel.SelectedCell = new Point(newX, newY);
                                                }
                                                break;
                                            case InventoryScreenCursorMode.ItemsFilter:
                                                if (newY < 0)
                                                {

                                                }
                                                else if (newX < 0)
                                                {
                                                    UIContainer.Instance.InventoryScreen.CursorMode = InventoryScreenCursorMode.Equipment;
                                                    UIContainer.Instance.InventoryScreen.EquipmentVisualModel.SelectedCell = new Point(2, 1);
                                                }
                                                else if (newY > 0)
                                                {
                                                    UIContainer.Instance.InventoryScreen.CursorMode = InventoryScreenCursorMode.PageSwitch;
                                                    UIContainer.Instance.InventoryScreen.GridModel.SelectedCell = new Point(0, 0);
                                                }
                                                else
                                                {
                                                    UIContainer.Instance.InventoryScreen.FiltersVisualModel.SelectedCell = new Point(newX, newY);
                                                }

                                                break;
                                            case InventoryScreenCursorMode.Equipment:

                                                if (newY >= 0 && newX >= 0 && newY <= 5)
                                                {
                                                    if (newY > 2)
                                                    {
                                                        if (newX == 0 || newX == 2)
                                                        {
                                                            newX = 1;
                                                        }
                                                    }
                                                    if (newX >= 3)
                                                    {
                                                        UIContainer.Instance.InventoryScreen.CursorMode = InventoryScreenCursorMode.Items;
                                                        UIContainer.Instance.InventoryScreen.GridModel.SelectedCell = new Point(0, 0);
                                                    }
                                                    else
                                                    {

                                                        var newP = new Point(newX, newY);
                                                        if (UIContainer.Instance.InventoryScreen.EquipmentVisualModel.CursorPositionsDict.TryGetValue(newP, out var slot))
                                                        {
                                                            UIContainer.Instance.InventoryScreen.EquipmentVisualModel.SelectedCell = new Point(newX, newY);
                                                        }
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                    break;

                                case IntentEnum.Interact:
                                    switch (UIContainer.Instance.InventoryScreen.CursorMode)
                                    {
                                        case InventoryScreenCursorMode.PageSwitch:
                                            if (UIContainer.Instance.InventoryScreen.PageSwitchModel.SelectedCell.X == 0)
                                            {
                                                UIContainer.Instance.InventoryScreen.PreviousPage();
                                            }
                                            else if (UIContainer.Instance.InventoryScreen.PageSwitchModel.SelectedCell.X == 1)
                                            {
                                                UIContainer.Instance.InventoryScreen.NextPage();
                                            }
                                            break;
                                        case InventoryScreenCursorMode.Items:
                                            {
                                                var p = UIContainer.Instance.InventoryScreen.GridModel.SelectedCell;
                                                var itemId = UIContainer.Instance.InventoryScreen.GridModel.CurrentPage.Cells[p.X, p.Y]?.ItemId;

                                                if (itemId.HasValue && itemId != Guid.Empty)
                                                {

                                                    var itemEntity = namelessGame.GetEntity(itemId.Value);

                                                    var equipmentComponent = itemEntity.GetComponentOfType<Equipment>();

                                                    var slot = equipmentComponent.PossibleSlots.First();
                                                    var playerEquipment = namelessGame.PlayerEntity.GetComponentOfType<EquipmentSlots>();
                                                    var slotTuple = playerEquipment.Slots.First(x => x.Item1 == slot);
                                                    EquipmentSlot equipmentSlot = null;
                                                    //unequip previous item if any
                                                    if (slotTuple.Item2.Equipment != null)
                                                    {
                                                        equipmentSlot = slotTuple.Item2;
                                                        var takeOffCommand = new EquipOrTakeOffCommand(equipmentSlot.Equipment.ParentEntityId, false);
                                                        namelessGame.Commander.EnqueueCommand(takeOffCommand);
                                                    }

                                                    var equipCommand = new EquipOrTakeOffCommand(itemEntity.Id, true, slot);
                                                    namelessGame.Commander.EnqueueCommand(equipCommand);

                                                }
                                            }
                                            break;
                                        case InventoryScreenCursorMode.ItemsFilter:
                                            {
                                                var flags = UIContainer.Instance.InventoryScreen.Flags;
                                                var index = UIContainer.Instance.InventoryScreen.FiltersVisualModel.SelectedCell.X;
                                                if (index >= 0 && index <= UIContainer.Instance.InventoryScreen.CountOfFilters)
                                                {
                                                    flags.FilterArray[index] = !flags.FilterArray[index];
                                                }
                                            }
                                            break;
                                        case InventoryScreenCursorMode.Equipment:
                                            {
                                                var slot = UIContainer.Instance.InventoryScreen.EquipmentVisualModel.CursorPositionsDict[UIContainer.Instance.InventoryScreen.EquipmentVisualModel.SelectedCell];
                                                var playerEquipment = namelessGame.PlayerEntity.GetComponentOfType<EquipmentSlots>();
                                                var slotTuple = playerEquipment.Slots.First(x => x.Item1 == slot);
                                                EquipmentSlot equipmentSlot = null;
                                                if (slotTuple.Item2.Equipment != null)
                                                {
                                                    equipmentSlot = slotTuple.Item2;
                                                    var takeOffCommand = new EquipOrTakeOffCommand(equipmentSlot.Equipment.ParentEntityId, false);
                                                    namelessGame.Commander.EnqueueCommand(takeOffCommand);
                                                }
                                            }
                                            break;
                                    }
                                    break;
                                case IntentEnum.Escape:
                                    BackToGame(namelessGame);
                                    break;
                                case IntentEnum.Add:
                                    UIContainer.Instance.InventoryScreen.NextPage();
                                    break;
                                case IntentEnum.Substract:
                                    UIContainer.Instance.InventoryScreen.PreviousPage();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    inputComponent.Intents.Clear();
                }


                bool updated = false;
                while (namelessGame.Commander.DequeueCommand<UpdateInventoryScreenCommand>(out var equipOrTakeOffCommand))
                {
                    if (!updated)
                    {
                        updated = true;
                        UIContainer.Instance.InventoryScreen.FillInventoryWithAll();
                    }
                }
            }
        }

        internal void BackToGame(NamelessGame game)
        {
            game.ContextToSwitch = ContextFactory.GetIngameContext(game);
        }

    }
}