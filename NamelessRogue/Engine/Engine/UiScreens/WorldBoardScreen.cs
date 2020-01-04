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
    public enum WorldBoardScreenAction
    {
        ReturnToGame,
        WorldMap,
        LocalMap,
        TerrainMode,
        RegionsMode,
        PoliticalMode,
        ArtifactMode,
    }
    public class WorldBoardScreen : BaseGuiScreen
    {
        public List<WorldBoardScreenAction> Actions { get; private set; } = new List<WorldBoardScreenAction>();
        public WorldBoardScreenAction Mode { get; set; }
        public WorldBoardScreen(NamelessGame game)
        {
            Panel = new Panel()
            {
                Width = (int)game.GetSettings().HudWidth(),
                Height = game.GetActualCharacterHeight(),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };


            var vPanel = new VerticalStackPanel();

            ReturnToGame = CreateButton("Back", game.GetSettings().HudWidth() - 50);
            ReturnToGame.Click += ReturnToGameOnClick;
            ModeTerrain = CreateButton("Terrain", game.GetSettings().HudWidth() - 50);
            ModeTerrain.Click += OnClickModeTerrain;

            ModeRegions = CreateButton("Regions", game.GetSettings().HudWidth() - 50);
            ModeRegions.Click += OnClickLandmasses;

            ModePolitical = CreateButton("Political", game.GetSettings().HudWidth() - 50);
            ModePolitical.Click += OnClickPolitical;

            ModeArtifacts = CreateButton("Artifacts", game.GetSettings().HudWidth() - 50);
            ModeArtifacts.Click += OnClickArtifacts;

            var grid = new Grid() { VerticalAlignment = VerticalAlignment.Top, ColumnSpacing = 3, Width = (int)game.GetSettings().HudWidth() - 50, HorizontalAlignment = HorizontalAlignment.Center};

            LocalMap = new ImageTextButton();
            LocalMap.Text = "Local";
            LocalMap.GridColumn = 0;
            LocalMap.Width = (int)game.GetSettings().HudWidth() / 2 - 50;
            LocalMap.Height = 50;
            LocalMap.Click += OnLocalMap;
            LocalMap.HorizontalAlignment = HorizontalAlignment.Left;
            LocalMap.VerticalAlignment = VerticalAlignment.Top;
            LocalMap.ContentHorizontalAlignment = HorizontalAlignment.Center;
            LocalMap.ContentVerticalAlignment = VerticalAlignment.Center;


            WorldMap = new ImageTextButton();
            WorldMap.Text = "World";
            WorldMap.GridColumn = 2;
            WorldMap.Width = (int)game.GetSettings().HudWidth() / 2 - 50;
            WorldMap.Height = 50;
            WorldMap.Click += OnWorldMap;
            WorldMap.HorizontalAlignment = HorizontalAlignment.Left;
            WorldMap.VerticalAlignment = VerticalAlignment.Top;
            WorldMap.ContentHorizontalAlignment = HorizontalAlignment.Center;
            WorldMap.ContentVerticalAlignment = VerticalAlignment.Center;


            grid.Widgets.Add(LocalMap);
            grid.Widgets.Add(WorldMap);

            TextBox list = new TextBox();
            list.Width = (int) game.GetSettings().HudWidth() - 50;
            list.Height = 100;
            list.Readonly = true;
            DescriptionLog = list;

            vPanel.Widgets.Add(grid);
            vPanel.Widgets.Add(new HorizontalSeparator());
            vPanel.Widgets.Add(DescriptionLog);
            vPanel.Widgets.Add(new HorizontalSeparator());
            vPanel.Widgets.Add(ModeTerrain);
            vPanel.Widgets.Add(ModeRegions);
            vPanel.Widgets.Add(ModePolitical);
            vPanel.Widgets.Add(ModeArtifacts);
            vPanel.Widgets.Add(ReturnToGame);

            Panel.Widgets.Add(vPanel);

            Desktop.Widgets.Add(Panel);
        }

        private void OnWorldMap(object sender, EventArgs e)
        {
            Actions.Add(WorldBoardScreenAction.WorldMap);
        }

        private void OnLocalMap(object sender, EventArgs e)
        {
            Actions.Add(WorldBoardScreenAction.LocalMap);
        }


        private void OnClickArtifacts(object sender, EventArgs e)
        {
            Actions.Add(WorldBoardScreenAction.ArtifactMode);
            Mode = WorldBoardScreenAction.ArtifactMode;
        }

        private void OnClickPolitical(object sender, EventArgs e)
        {
            Actions.Add(WorldBoardScreenAction.PoliticalMode);
            Mode = WorldBoardScreenAction.PoliticalMode;
        }

        private void OnClickLandmasses(object sender, EventArgs e)
        {
            Actions.Add(WorldBoardScreenAction.RegionsMode);
            Mode = WorldBoardScreenAction.RegionsMode;
        }

        private void OnClickModeTerrain(object sender, EventArgs e)
        {
            Actions.Add(WorldBoardScreenAction.TerrainMode);
            Mode = WorldBoardScreenAction.TerrainMode;
        }

        private ImageTextButton CreateButton(string Text, float width)
        {
           
            var result = new ImageTextButton()
            {
                ContentHorizontalAlignment = HorizontalAlignment.Center,
                ContentVerticalAlignment = VerticalAlignment.Center,
                Text = Text,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = (int)width,
                Height = 50
            };



            return result;
        }

        private void ReturnToGameOnClick(object sender, EventArgs e)
        {
            Actions.Add(WorldBoardScreenAction.ReturnToGame);
        }
        public TextBox DescriptionLog { get; private set; }

        public ImageTextButton ModeTerrain { get; set; }
        public ImageTextButton ModeRegions { get; set; }
        public ImageTextButton ModePolitical { get; set; }
        public ImageTextButton ModeArtifacts { get; set; }
        public ImageTextButton ReturnToGame { get; set; }

        public ImageTextButton LocalMap { get; set; }
        public ImageTextButton WorldMap { get; set; }

    }
}
