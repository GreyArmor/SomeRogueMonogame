using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeonBit.UI;
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
        public List<MainMenuAction> Actions { get; } = new List<MainMenuAction>();
        public Button NewGame { get; private set; }
        public Button LoadGame { get; private set; }
        public Button CreateTimeline { get; set; }
        public Button Options { get; set; }
        public Button Exit { get; }

        public MainMenuScreen(NamelessGame game)
        {

            Panel = new Panel(new Vector2(game.GetSettings().HudWidth(), game.GetActualCharacterHeight()), PanelSkin.Default, Anchor.Center);

            NewGame = new Button("New game");
            LoadGame = new Button("Load");
            CreateTimeline = new Button("Create timeline");
            Options = new Button("Options");
            Exit = new Button("Exit");

            NewGame.OnClick += entity => { Actions.Add(MainMenuAction.NewGame); };
            LoadGame.OnClick += entity => { Actions.Add(MainMenuAction.LoadGame); };
            CreateTimeline.OnClick += entity => { Actions.Add(MainMenuAction.GenerateNewTimeline); };
            Options.OnClick += entity => { Actions.Add(MainMenuAction.Options); };
            Exit.OnClick += entity => { Actions.Add(MainMenuAction.Exit); };

            Panel.AddChild(NewGame);
            Panel.AddChild(LoadGame);
            Panel.AddChild(CreateTimeline);
            Panel.AddChild(Options);
            Panel.AddChild(Exit);

            UserInterface.Active.AddEntity(Panel);
        }
    }
}
