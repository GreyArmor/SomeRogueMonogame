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
                            case IntentEnum.ConetextualHoteyPressed:
                                //TODO add contextual hotkeys logic here
                                break;
                            case IntentEnum.Enter:
                            {
                               UiFactory.InventoryScreen.SelectedTable.OnItemClick.Invoke();
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