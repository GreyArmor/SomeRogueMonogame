using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Engine.Components.UI;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.UiScreens
{
    public enum InventoryScreenAction
    {
        ReturnToGame
    }
    public class InventoryScreen : BaseGuiScreen
    {
        private NamelessGame game;

        public ImageTextButton ReturnToGame { get; set; }

        public List<InventoryScreenAction> Actions { get; set; } = new List<InventoryScreenAction>();

        public InventoryScreen(NamelessGame game)
        {
            this.game = game;

            Panel = new Panel()
            {
                Width = game.GetActualWidth(),
                Height = game.GetActualCharacterHeight(),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };
            ReturnToGame = new ImageTextButton()
            {
                GridColumn = 2,
                ContentHorizontalAlignment = HorizontalAlignment.Center,
                ContentVerticalAlignment = VerticalAlignment.Center,
                Text = "Back",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = 200,
                Height = 50
            };
            ReturnToGame.Click += OnClickReturnToGame;

            Panel.Widgets.Add(ReturnToGame);
            Desktop.Widgets.Add(Panel);

        }

        private void OnClickReturnToGame(object sender, EventArgs e)
        {
            Actions.Add(InventoryScreenAction.ReturnToGame);
        }
    }
}
