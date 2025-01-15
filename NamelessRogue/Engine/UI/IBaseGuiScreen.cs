using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NamelessRogue.Engine.UI
{
	public interface IBaseGuiScreen
	{
        Vector2 UiSize { get; set; }

        int CurrentOptionsItem { get; set; }

        void CloseOptionsPopUp();
        public void DrawLayout();
        void DrawOptionsPopup();
        void OpenOptionsPopUp(IEnumerable<string> options, Vector2 position);
    }
}
