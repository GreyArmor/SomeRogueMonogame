using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace NamelessRogue.Engine.Engine.Input
{
    public class KeyIntentTraslator {
        public static List<Intent> Translate(Keys[] keyCodes)
        {
            List<Intent> result = new List<Intent>();
            ////TODO: Add dictionary for actions, based on game config files

            if (keyCodes.Length == 0)
            {}
            else if (keyCodes.Length > 1)
            {}
            else
            {
                var keyCode = keyCodes[0];
                switch (keyCode)
                {
                    case Keys.Up:
                    case Keys.NumPad8:
                    case Keys.W:
                        result.Add(Intent.MoveUp);
                        break;
                    case Keys.NumPad2:
                    case Keys.Down:
                    case Keys.X:
                        result.Add(Intent.MoveDown);
                        break;
                    case Keys.NumPad4:
                    case Keys.Left:
                    case Keys.A:
                        result.Add(Intent.MoveLeft);
                        break;
                    case Keys.NumPad6:
                    case Keys.Right:
                    case Keys.D:
                        result.Add(Intent.MoveRight);
                        break;
                    case Keys.NumPad7:
                    case Keys.Q:
                        result.Add(Intent.MoveTopLeft);
                        break;
                    case Keys.NumPad9:
                    case Keys.E:
                        result.Add(Intent.MoveTopRight);
                        break;
                    case Keys.NumPad1:
                    case Keys.Z:
                        result.Add(Intent.MoveBottomLeft);
                        break;
                    case Keys.NumPad3:
                    case Keys.C:
                        result.Add(Intent.MoveBottomRight);
                        break;
                    case Keys.F:
                        result.Add(Intent.LookAtMode);
                        break;
                    case Keys.NumPad5:
                    case Keys.S:
                        result.Add(Intent.SkipTurn);
                        break;
                    case Keys.OemComma:
                        result.Add(Intent.PickUpItem);
                        break;;

                }
            }

            return result;
		
        }
    }
}
