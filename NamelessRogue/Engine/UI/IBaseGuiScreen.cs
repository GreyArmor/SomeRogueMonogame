using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NamelessRogue.Engine.UI
{
	public interface IBaseGuiScreen
	{
        Vector2 UiSize { get; set; }

        public void DrawLayout();
    }
}
