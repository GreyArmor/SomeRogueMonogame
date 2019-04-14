using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            {
                if (keyCodes.Contains(Keys.W) && keyCodes.Contains(Keys.A))
                {
                    result.Add(Intent.MoveTopLeft);
                }
                else if (keyCodes.Contains(Keys.W) && keyCodes.Contains(Keys.D))
                {
                    result.Add(Intent.MoveTopRight);
                }
                else if (keyCodes.Contains(Keys.S) && keyCodes.Contains(Keys.A))
                {
                    result.Add(Intent.MoveBottomLeft);
                }
                else if (keyCodes.Contains(Keys.S) && keyCodes.Contains(Keys.D))
                {
                    result.Add(Intent.MoveBottomRight);
                }
            }
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
                    case Keys.S:
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
                        result.Add(Intent.MoveTopLeft);
                        break;
                    case Keys.NumPad9:
                        result.Add(Intent.MoveTopRight);
                        break;
                    case Keys.NumPad1:
                        result.Add(Intent.MoveBottomLeft);
                        break;
                    case Keys.NumPad3:
                        result.Add(Intent.MoveBottomRight);
                        break;
                    case Keys.F:
                        result.Add(Intent.LookAtMode);
                        break;
                    case Keys.NumPad5:
                        result.Add(Intent.SkipTurn);
                        break;
                    case Keys.P:
                        result.Add(Intent.PickUpItem);
                        break;
                    case Keys.Enter:
                        result.Add(Intent.Enter);
                        break;



                }
            }

            return result;
		
        }
    }
}
