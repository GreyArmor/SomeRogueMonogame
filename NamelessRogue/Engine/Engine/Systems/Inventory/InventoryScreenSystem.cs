using System;
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

            //foreach (IEntity entity in namelessGame.GetEntities())
            //{
            //    InputComponent inputComponent = entity.GetComponentOfType<InputComponent>();
            //    if (inputComponent != null)
            //    {
            //        var playerEntity = namelessGame.GetEntitiesByComponentClass<Player>().First();
            //        foreach (Intent intent in inputComponent.Intents)
            //        {
            //            switch (intent)
            //            {
            //                case Intent.MoveDown:
            //                    {
            //                        int nextIndex = UiFactory.InventoryScreen.PickableItemList.SelectedItemIndex + 1;
            //                        if (nextIndex >= UiFactory.InventoryScreen.PickableItemList.Items.Count)
            //                        {
            //                            nextIndex = 0;
            //                        }

            //                        UiFactory.InventoryScreen.PickableItemList.Select(nextIndex);
            //                        break;
            //                    }
            //                case Intent.MoveUp:
            //                    {
            //                        int nextIndex = UiFactory.InventoryScreen.PickableItemList.SelectedItemIndex - 1;
            //                        if (nextIndex < 0)
            //                        {
            //                            nextIndex = UiFactory.InventoryScreen.PickableItemList.Items.Count - 1;
            //                        }

            //                        UiFactory.InventoryScreen.PickableItemList.Select(nextIndex);
            //                        break;
            //                    }

            //                case Intent.Enter:
            //                    {

            //                    }
            //                    break;
            //                default:
            //                    break;
            //            }
            //        }
            //        inputComponent.Intents.Clear();
            //    }
            //}



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
