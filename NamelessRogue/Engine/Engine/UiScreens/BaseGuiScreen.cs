using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeonBit.UI;
using GeonBit.UI.Entities;
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
            //UserInterface.Active.AddEntity(Panel);
        }

        public void Hide()
        {
            Panel.Visible = false;
            Panel.Enabled = false;
         //   Panel.
        }
    }
}
