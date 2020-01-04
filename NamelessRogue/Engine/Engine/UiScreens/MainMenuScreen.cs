using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
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
        public ImageTextButton NewGame { get; private set; }
        public ImageTextButton LoadGame { get; private set; }
        public ImageTextButton CreateTimeline { get; set; }
        public ImageTextButton Options { get; set; }
        public ImageTextButton Exit { get; }

        public MainMenuScreen(NamelessGame game)
        {


            // Panel = new Panel(new Vector2(game.GetSettings().HudWidth(), game.GetActualCharacterHeight()), PanelSkin.Default, Anchor.Center);

            Panel = new Panel(){HorizontalAlignment = HorizontalAlignment.Center};
            var vPanel = new VerticalStackPanel();
            NewGame = new ImageTextButton(){Text = "New game", Width = 200 , Height =  50};
            LoadGame = new ImageTextButton() { Text = "Load", Width = 200, Height = 50 }; ;
            CreateTimeline = new ImageTextButton() { Text = "Create timeline", Width = 200, Height = 50 };
            Options = new ImageTextButton() { Text = "Options", Width = 200, Height = 50 };
            Exit = new ImageTextButton(){Text = "Exit", Width = 200, Height = 50 };

            NewGame.Click += (sender, args) => { Actions.Add(MainMenuAction.NewGame); };
            LoadGame.Click += (sender, args) => { Actions.Add(MainMenuAction.LoadGame); };
            CreateTimeline.Click += (sender, args) => { Actions.Add(MainMenuAction.GenerateNewTimeline); };
            Options.Click += (sender, args) => { Actions.Add(MainMenuAction.Options); };
            Exit.Click += (sender, args) => { Actions.Add(MainMenuAction.Exit); };

            vPanel.Widgets.Add(NewGame);
            vPanel.Widgets.Add(LoadGame);
            vPanel.Widgets.Add(CreateTimeline);
            vPanel.Widgets.Add(Options);
            vPanel.Widgets.Add(Exit);
            Panel.Widgets.Add(vPanel);
            Desktop.Widgets.Add(Panel);
        }
    }
}
