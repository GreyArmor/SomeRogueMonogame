using System;
using System.Collections.Generic;

using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Systems.MainMenu
{
    public class MainMenuScreenSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame game)
        {
                switch (UIController.Instance.MainMenu.Action)
                {
                    case MainMenuAction.GenerateNewTimeline:
                        game.ContextToSwitch = ContextFactory.GetWorldGenContext(game);
                        break;
                    case MainMenuAction.NewGame:
                        game.ContextToSwitch = ContextFactory.GetIngameContext(game);
                        break;
                    case MainMenuAction.Options:
                        break;
                    case MainMenuAction.LoadGame:
                        break;
                    case MainMenuAction.Exit:
                        game.Exit();
                        break;
                    default:
                        break;
                }

            UIController.Instance.MainMenu.Action = MainMenuAction.None;
        }
    }

	
}
