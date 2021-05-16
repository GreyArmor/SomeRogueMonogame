using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;

namespace NamelessRogue.Engine.UiScreens.UI
{
    public class ScrollableListBox : ListBox
    {
        public ScrollViewer Scroll
        {
            get { return this.InternalChild; }
        }
    }
}
