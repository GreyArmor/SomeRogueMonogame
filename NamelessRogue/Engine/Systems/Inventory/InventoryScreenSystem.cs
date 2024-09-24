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

                            var position = UIContainer.Instance.InventoryScreen.SelectedCell;
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

                                        switch(UIContainer.Instance.InventoryScreen.CursorMode)
                                        {
                                            case InventoryScreenCursorMode.Items:

                                                if (newY < 0)
                                                {
                                                    UIContainer.Instance.InventoryScreen.CursorMode = InventoryScreenCursorMode.ItemsFilter;
                                                    UIContainer.Instance.InventoryScreen.SelectedCell = new Point(0, 0);
                                                }
                                                else
                                                {
                                                    UIContainer.Instance.InventoryScreen.SelectedCell = new Point(newX, newY);
                                                }
                                                break;
                                            case InventoryScreenCursorMode.ItemsFilter:
                                                if (newY < 0)
                                                {

                                                }
                                                else if (newX < 0)
                                                {
                                                  //  UIContainer.Instance.InventoryScreen.CursorMode = InventoryScreenCursorMode.Equipment;
                                                 //   UIContainer.Instance.InventoryScreen.SelectedCell = new Point(0, 0);
                                                }
                                                else if(newY>0)
                                                {
                                                      UIContainer.Instance.InventoryScreen.CursorMode = InventoryScreenCursorMode.Items;
                                                      UIContainer.Instance.InventoryScreen.SelectedCell = new Point(0, 0);
                                                }
                                                else
                                                {
                                                    UIContainer.Instance.InventoryScreen.SelectedCell = new Point(newX, newY);
                                                }

                                                break;
                                            case InventoryScreenCursorMode.Equipment:
                                                break;
                                        }

                                       
                                    }
                                    break;
        
                                case IntentEnum.Interact:
                                    switch (UIContainer.Instance.InventoryScreen.CursorMode)
                                    {
                                        case InventoryScreenCursorMode.Items:
                                            break;
                                        case InventoryScreenCursorMode.ItemsFilter:
                                            {
                                                var flags = UIContainer.Instance.InventoryScreen.Flags;
                                                var index = UIContainer.Instance.InventoryScreen.SelectedCell.X;
                                                if (index >= 0 && index <= UIContainer.Instance.InventoryScreen.CountOfFilters)
                                                {
                                                    flags.FilterArray[index] = !flags.FilterArray[index];
                                                }
                                            }
                                            break;
                                        case InventoryScreenCursorMode.Equipment: 
                                            break;
                                       
                                    }
                                    break;
                                case IntentEnum.Enter:
                                    {
                                    //    UIController.InventoryScreen.SelectedTable.OnItemClick.Invoke(UIController.InventoryScreen
                                    //        .SelectedTable.SelectedItem);
                                    }
                                    break;
                                case IntentEnum.Escape:
                                    namelessGame.ContextToSwitch = ContextFactory.GetIngameContext(namelessGame);
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