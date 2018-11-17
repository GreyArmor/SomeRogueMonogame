using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Factories;
using NamelessRogue.Engine.Engine.UiScreens;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems
{
    public class WorldBoardScreenSystem : ISystem
    {
        public void Update(long gameTime, NamelessGame namelessGame)
        {
            foreach (var worldBoardScreenAction in UiFactory.WorldBoardScreen.Actions)
            {
                switch (worldBoardScreenAction)
                {
                    case WorldBoardScreenAction.ReturnToGame:

                        namelessGame.ContextToSwitch = ContextFactory.GetIngameContext(namelessGame);
                        break;
                    default:
                        break;
                }
            }
            UiFactory.WorldBoardScreen.Actions.Clear();
        }
    }
}

