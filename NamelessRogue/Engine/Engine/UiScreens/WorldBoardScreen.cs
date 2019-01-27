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
        RegionsMode,
        PoliticalMode,
        ArtifactMode,
        ResourceMode
    }
    public class WorldBoardScreen : BaseGuiScreen
    {
        public List<WorldBoardScreenAction> Actions { get; private set; } = new List<WorldBoardScreenAction>();
        public WorldBoardScreenAction Mode { get; set; }
        public WorldBoardScreen(NamelessGame game)
        {
            Panel = new Panel(new Vector2(game.GetSettings().HudWidth(), game.GetActualCharacterHeight()), PanelSkin.Default, Anchor.BottomRight);
            ReturnToGame = CreateButton("Back", game.GetSettings().HudWidth()-50);
            ReturnToGame.OnClick += ReturnToGameOnClick;

            ModeTerrain = CreateButton("Terrain", game.GetSettings().HudWidth()-50);
            ModeTerrain.OnClick += OnClickModeTerrain;

            ModeRegions = CreateButton("Regions", game.GetSettings().HudWidth()-50);
            ModeRegions.OnClick += OnClickLandmasses;

            ModePolitical = CreateButton("Political", game.GetSettings().HudWidth() - 50);
            ModePolitical.OnClick += OnClickPolitical;

            ModeArtifacts = CreateButton("Artifacts", game.GetSettings().HudWidth() - 50);
            ModeArtifacts.OnClick += OnClickArtifacts;
            ModeResource = CreateButton("Resources", game.GetSettings().HudWidth() - 50);
            ModeResource.OnClick += OnClickResource;

            SelectList list = new SelectList(new Vector2(0, 150));
            list.Locked = true;
            list.ItemsScale = 0.5f;
            list.ExtraSpaceBetweenLines = -10;
            DescriptionLog = list;

            Panel.AddChild(DescriptionLog);

            Panel.AddChild(ModeTerrain);
            Panel.AddChild(ModeRegions);
            Panel.AddChild(ModePolitical);
            Panel.AddChild(ModeArtifacts);
            Panel.AddChild(ModeResource);
            Panel.AddChild(ReturnToGame);
            
            UserInterface.Active.AddEntity(Panel);
        }

        private void OnClickResource(Entity entity)
        {
            Actions.Add(WorldBoardScreenAction.ResourceMode);
            Mode = WorldBoardScreenAction.ResourceMode;
        }

        public Button ModeResource { get; set; }

        private void OnClickArtifacts(Entity entity)
        {
            Actions.Add(WorldBoardScreenAction.ArtifactMode);
            Mode = WorldBoardScreenAction.ArtifactMode;
        }

        private void OnClickPolitical(Entity entity)
        {
            Actions.Add(WorldBoardScreenAction.PoliticalMode);
            Mode = WorldBoardScreenAction.PoliticalMode;
        }

        private void OnClickLandmasses(Entity entity)
        {
            Actions.Add(WorldBoardScreenAction.RegionsMode);
            Mode = WorldBoardScreenAction.RegionsMode;
        }

        private void OnClickModeTerrain(Entity entity)
        {
            Actions.Add(WorldBoardScreenAction.TerrainMode);
            Mode = WorldBoardScreenAction.TerrainMode;
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
        public SelectList DescriptionLog { get; private set; }

        public Button ModeTerrain { get; set; }
        public Button ModeRegions { get; set; }
        public Button ModePolitical { get; set; }
        public Button ModeArtifacts { get; set; }
        public Button ReturnToGame { get; set; }

       

    }
}
