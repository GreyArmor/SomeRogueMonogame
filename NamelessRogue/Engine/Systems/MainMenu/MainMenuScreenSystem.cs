using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.UI;
using NamelessRogue.Engine.UI.GumScreens.Commands.MainMenu;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;

namespace NamelessRogue.Engine.Systems.MainMenu
{
    public class MainMenuScreenSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame game)
        {
            switch (UIContainer.Instance.MainMenu.Action)
            {
                case MainMenuAction.GenerateNewTimeline:
                    game.ContextToSwitch = ContextFactory.GetWorldGenContext(game);
                    break;
                case MainMenuAction.NewGame:
                    game.ContextToSwitch = ContextFactory.GetNewGamePickWorldContext(game);
                    break;
                case MainMenuAction.Options:
                    break;
                case MainMenuAction.LoadGame:
                    break;
                case MainMenuAction.Editors:
                    game.ContextToSwitch = ContextFactory.GetEditorsPickerContext(game);
                    break;
                case MainMenuAction.Exit:
                    game.Exit();
                    break;
                default:
                    break;
            }

            UIContainer.Instance.MainMenu.Action = MainMenuAction.None;

            if (game.Commander.DequeueCommand(out MainMenuNewGameCommand _)) { game.ContextToSwitch = ContextFactory.GetNewGamePickWorldContext(game); }
            if (game.Commander.DequeueCommand(out MainMenuWorldGenCommand _)) { game.ContextToSwitch = ContextFactory.GetWorldGenContext(game); }
			if (game.Commander.DequeueCommand(out MainMenuLoadGameCommand _)) { /**/ }
			if (game.Commander.DequeueCommand(out MainMenuOptionCommand _)) { /**/ }
			if (game.Commander.DequeueCommand(out MainMenuEditorsCommand _)) { game.ContextToSwitch = ContextFactory.GetEditorsPickerContext(game); }
			if (game.Commander.DequeueCommand(out MainMenuExitCommand _)) { game.Exit(); }


		}
    }


}
