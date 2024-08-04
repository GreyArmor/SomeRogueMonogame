

using NamelessRogue.Engine.Infrastructure;
using System.Collections.Generic;
using System.Windows.Forms;



namespace NamelessRogue.Engine.Input
{
    public enum IntentEnum {
        None,
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveTopLeft,
        MoveTopRight,
        MoveBottomLeft,
        MoveBottomRight,
        MoveRight,
        PickUpItem,
        LookAtMode, DropItem,
        SkipTurn,
        Enter,
        Escape,
        ConetextualHotkeyPressed,
        Quicksave,
        Quickload,
        ZoomIn,
        ZoomOut,
        MouseChanged,
	}

    public class Intent
    {
        public Intent(List<Key> pressedKey, char pressedChar)
        {
            this.PressedKey = pressedKey;
            this.PressedChar = pressedChar;
        }
        public List<Key> PressedKey { get; set; }
        public char PressedChar { get; set; }
        public IntentEnum Intention { get; set; }

    }
}
