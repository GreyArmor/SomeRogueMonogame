using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace NamelessRogue.Engine.Input
{
    public class IngameKeyIntentTraslator : IKeyIntentTraslator
    {
        public virtual List<Intent> Translate(Keys[] keyCodes, char lastCommand, MouseState mouseState)
        {
            List<Intent> result = new List<Intent>();
            ////TODO: Add dictionary for actions, based on game config files

            if (keyCodes.Length == 0)
            {
            }
            else
            {
                for (int i = 0; i < keyCodes.Length; i++)
                {
                    var keyCode = keyCodes[i];
                    Intent intent = new Intent(keyCodes.ToList(), lastCommand);
                    result.Add(intent);
                    switch (keyCode)
                    {
                        case Keys.Up:
                        case Keys.W:
                        case Keys.NumPad8:
                            intent.Intention = IntentEnum.MoveUp;
                            break;
                        case Keys.S:
                        case Keys.Down:
                        case Keys.NumPad2:
                            intent.Intention = IntentEnum.MoveDown;
                            break;
                        case Keys.A:
                        case Keys.Left:
                        case Keys.NumPad4:
                            intent.Intention = IntentEnum.MoveLeft;
                            break;
                        case Keys.D:
                        case Keys.Right:
                        case Keys.NumPad6:
                            intent.Intention = IntentEnum.MoveRight;
                            break;
                        case Keys.NumPad7:
                            intent.Intention = IntentEnum.MoveTopLeft;
                            break;
                        case Keys.NumPad9:
                            intent.Intention = IntentEnum.MoveTopRight;
                            break;
                        case Keys.NumPad1:
                            intent.Intention = IntentEnum.MoveBottomLeft;
                            break;
                        case Keys.NumPad3:
                            intent.Intention = IntentEnum.MoveBottomRight;
                            break;
                        case Keys.NumPad5:
                            intent.Intention = IntentEnum.SkipTurn;
                            break;
                        case Keys.Enter:
                            intent.Intention = IntentEnum.Enter;
                            break;
                        case Keys.P:
                            intent.Intention = IntentEnum.PickUpItem;
                            break;
                        case Keys.F5:
                            intent.Intention = IntentEnum.Quicksave;
                            break;
                        case Keys.F9:
                            intent.Intention = IntentEnum.Quickload;
                            break;
                        case Keys.Z:
                            if (lastCommand == 'z')
                            {
                                intent.Intention = IntentEnum.ZoomOut;
                            }
                            else if (lastCommand == 'Z')
                            {
                                intent.Intention = IntentEnum.ZoomIn;
                            }
                            break;
                        case Keys.L:
                            {
                                intent.Intention = IntentEnum.LookAtMode;
                            }
                            break;
                        case Keys.F:
                            {
                                intent.Intention = IntentEnum.Fire;
                            }
                            break;
                        case Keys.OemComma:
                            {
                                if (lastCommand == '<')
                                {
                                    intent.Intention = IntentEnum.MoveAscent;
                                }
                            }
                            break;

                        case Keys.OemPeriod:
                            {
                                if (lastCommand == '>')
                                {
                                    intent.Intention = IntentEnum.MoveDescent;
                                }
                            }
                            break;
                        case Keys.Escape:
                            intent.Intention = IntentEnum.Escape;
                            break;
                        case Keys.Tab:
                            intent.Intention = IntentEnum.SwitchTarget;
                            break;
                        default:
                            break;
                    }
                }
            }

            Intent mouseIntent = new Intent(keyCodes.ToList(), lastCommand);
			mouseIntent.Intention = IntentEnum.MouseChanged;
            result.Add(mouseIntent);

            return result;
        }
    }
}
