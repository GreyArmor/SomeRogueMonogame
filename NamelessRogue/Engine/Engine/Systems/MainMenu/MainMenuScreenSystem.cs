﻿using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Factories;
using NamelessRogue.Engine.Engine.UiScreens;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems.MainMenu
{
    public class MainMenuScreenSystem : ISystem
    {
        public void Update(long gameTime, NamelessGame namelessGame)
        {
            foreach (var action in UiFactory.MainMenuScreen.Actions)
            {
                switch (action)
                {
                    case MainMenuAction.GenerateNewTimeline:
                        break;
                    case MainMenuAction.NewGame:
                        namelessGame.ContextToSwitch = ContextFactory.GetIngameContext(namelessGame);
                        break;
                    case MainMenuAction.Options:
                        break;
                    case MainMenuAction.LoadGame:
                        break;
                    case MainMenuAction.Exit:
                        namelessGame.Exit();
                        break;
                    default:
                        break;
                }
            }

            UiFactory.MainMenuScreen.Actions.Clear();
        }
    }
}
