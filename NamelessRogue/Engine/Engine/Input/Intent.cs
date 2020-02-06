 

using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace NamelessRogue.Engine.Engine.Input
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
        ConetextualHoteyPressed
    }

    public class Intent
    {
        public Intent(List<Keys> pressedKeys, char pressedChar)
        {
            this.PressedKeys = pressedKeys;
            this.PressedChar = pressedChar;
        }
        public List<Keys> PressedKeys { get; set; }
        public char PressedChar { get; set; }
        public IntentEnum Intention { get; set; }

    }
}
