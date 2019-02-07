using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.UiScreens
{

    public enum MainMenuAction
    {
        NewGame,
        LoadGame,
        GenerateNewTimeline,
        Options,
        Exit,
        
    }
    public class MainMenuScreen : BaseGuiScreen
    {
        public List<MainMenuAction> Actions { get; }
        public Button NewGame { get; private set; }
        public Button LoadGame { get; private set; }
        public Button CreateTimeline { get; set; }
        public Button Options { get; set; }
        public Button Exit { get; }

        public MainMenuScreen(NamelessGame game)
        {
            NewGame = new Button("New game", size: new Vector2(game.GetSettings().HudWidth() / 2 - 50, 50), anchor: Anchor.Center);
            LoadGame = new Button("Load", size: new Vector2(game.GetSettings().HudWidth() / 2 - 50, 50), anchor: Anchor.Center);
            CreateTimeline = new Button("Create timeline", size: new Vector2(game.GetSettings().HudWidth() / 2 - 50, 50), anchor: Anchor.Center);
            Options = new Button("Options", size: new Vector2(game.GetSettings().HudWidth() / 2 - 50, 50), anchor: Anchor.Center);
            Exit = new Button("Exit", size: new Vector2(game.GetSettings().HudWidth() / 2 - 50, 50), anchor: Anchor.Center);

            NewGame.OnClick += entity => {  };
            LoadGame.OnClick += entity => { };
            CreateTimeline.OnClick += entity => { };
            Options.OnClick += entity => { Actions.Add(MainMenuAction.Options); };
            Exit.OnClick += entity => { Actions.Add(MainMenuAction.Exit); };
        }
    }
}
