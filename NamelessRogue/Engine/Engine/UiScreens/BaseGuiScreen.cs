using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Myra.Graphics2D.UI;
using NamelessRogue.Engine.Engine.Factories;

namespace NamelessRogue.Engine.Engine.UiScreens
{
    public class BaseGuiScreen
    {
        public Panel Panel { get; protected set; }

        public void Show()
        {
            Panel.Visible = true;
            Panel.Enabled = true;
        }

        public void Hide()
        {
            Panel.Visible = false;
            Panel.Enabled = false;
        }
    }
}
