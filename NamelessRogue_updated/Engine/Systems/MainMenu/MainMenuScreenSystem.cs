using System;
using System.Collections.Generic;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.UiScreens;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems.MainMenu
{
    public class MainMenuScreenSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(long gameTime, NamelessGame namelessGame)
        {
            foreach (var action in UiFactory.MainMenuScreen.SimpleActions)
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

            UiFactory.MainMenuScreen.SimpleActions.Clear();
        }
    }
}
