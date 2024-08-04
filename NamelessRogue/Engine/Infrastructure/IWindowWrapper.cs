using NamelessRogue.Engine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Infrastructure
{
    public interface IWindowWrapper
    {
        string WindowName { get; set; }
        int WindowWidth { get; set; }
        int WindowHeight { get; set; }
        bool Fullscreen { get; set; }
        Point Position { get; set; }
        KeyboardState KeyboardState { get; set; }
        MouseState MouseState { get; set; }
        void Hide();
        void Show(int x, int y);

        bool SetIcon(string iconPath);

        void Init(int width, int height, int posX, int posY, string name, bool isFullScreen, string iconPath = null);

        void Update();
    }
}
