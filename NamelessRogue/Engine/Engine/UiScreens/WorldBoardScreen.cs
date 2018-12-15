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
        ReturnToGame
    }
    public class WorldBoardScreen : BaseGuiScreen
    {
        public List<WorldBoardScreenAction> Actions { get; private set; } = new List<WorldBoardScreenAction>();

        public WorldBoardScreen(NamelessGame game)
        {
            Panel = new Panel(new Vector2(game.GetSettings().HudWidth(), game.GetActualCharacterHeight()), PanelSkin.Default, Anchor.BottomRight);
            ReturnToGame = CreateButton("Back");
            ReturnToGame.OnClick += ReturnToGameOnClick;
            Panel.AddChild(ReturnToGame);
            UserInterface.Active.AddEntity(Panel);
        }

        private Button CreateButton(string Text)
        {
            var result = new Button(Text, size: new Vector2(150, 50), anchor: Anchor.BottomCenter);
            result.ButtonParagraph.Scale = 0.7f;
            return result;
        }

        private void ReturnToGameOnClick(Entity entity)
        {
            Actions.Add(WorldBoardScreenAction.ReturnToGame);
        }

        public Button ReturnToGame { get; set; }

    }
}
