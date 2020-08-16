using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Engine.UiScreens
{
    public interface IBaseGuiScreen
    {
        Panel Panel { get; }

        void Show();

        void Hide();
    }
}
