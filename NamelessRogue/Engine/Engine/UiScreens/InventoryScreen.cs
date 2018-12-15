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
    public class InventoryScreen : BaseGuiScreen
    {
        public InventoryScreen(NamelessGame game)
        {
            Panel = new Panel(new Vector2(game.GetActualWidth(), game.GetActualHeight()), PanelSkin.Default, Anchor.BottomRight);

        }
    }
}
