using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;

namespace NamelessRogue.Engine.Engine.UiScreens.UI
{
    public class ScrollableListBox : ListBox
    {
        public ScrollViewer Scroll
        {
            get { return this.InternalChild; }
        }

        public void ScrollToSelectedItem()
        {
           // EnsureVisible(8);
            
            //Scroll.ScrollPosition = new Point(0, SelectedItem.Widget.ActualBounds.Y);
        }
    }
}
