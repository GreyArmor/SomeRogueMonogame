using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Factories;
using NamelessRogue.Engine.Engine.UiScreens;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems.Inventory
{
    public class InventoryScreenSystem : ISystem
    {
        public void Update(long gameTime, NamelessGame namelessGame)
        {
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
