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
    public enum WorldBoardScreenAction
    {
        ReturnToGame,
        TerrainMode,
        RegionsMode
    }
    public class WorldBoardScreen : BaseGuiScreen
    {
        public List<WorldBoardScreenAction> Actions { get; private set; } = new List<WorldBoardScreenAction>();

        public WorldBoardScreen(NamelessGame game)
        {
            Panel = new Panel(new Vector2(game.GetSettings().HudWidth(), game.GetActualCharacterHeight()), PanelSkin.Default, Anchor.BottomRight);
            ReturnToGame = CreateButton("Back", game.GetSettings().HudWidth()-50);
            ReturnToGame.OnClick += ReturnToGameOnClick;

            ModeTerrain = CreateButton("Terrain", game.GetSettings().HudWidth()-50);
            ModeTerrain.OnClick += OnClickModeTerrain;

            ModeRegions = CreateButton("Regions", game.GetSettings().HudWidth()-50);
            ModeRegions.OnClick += OnClickLandmasses;

            Panel.AddChild(ModeTerrain);
            Panel.AddChild(ModeRegions);
            Panel.AddChild(ReturnToGame);
            UserInterface.Active.AddEntity(Panel);
        }

        private void OnClickLandmasses(Entity entity)
        {
            Actions.Add(WorldBoardScreenAction.RegionsMode);
        }

        private void OnClickModeTerrain(Entity entity)
        {
            Actions.Add(WorldBoardScreenAction.TerrainMode);
        }

        private Button CreateButton(string Text, float width)
        {
            var result = new Button(Text, size: new Vector2(width, 50), anchor: Anchor.Auto);
            result.ButtonParagraph.Scale = 0.7f;
            return result;
        }

        private void ReturnToGameOnClick(Entity entity)
        {
            Actions.Add(WorldBoardScreenAction.ReturnToGame);
        }


        public Button ModeTerrain { get; set; }
        public Button ModeRegions { get; set; }
        public Button ReturnToGame { get; set; }

       

    }
}
