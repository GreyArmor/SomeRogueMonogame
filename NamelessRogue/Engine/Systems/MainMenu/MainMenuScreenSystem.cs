using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems.MainMenu
{
    public class MainMenuScreenSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
                switch (UIContainer.Instance.MainMenu.Action)
                {
                    case MainMenuAction.GenerateNewTimeline:
                        namelessGame.ContextToSwitch = ContextFactory.GetWorldGenContext(namelessGame);
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

            UIContainer.Instance.MainMenu.Action = MainMenuAction.None;
        }
    }

	
}
