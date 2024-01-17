using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Veldrid;


namespace NamelessRogue.Engine.Input
{
    public class IngameKeyIntentTraslator : IKeyIntentTraslator
    {
        public virtual List<Intent> Translate(Key[] keyCodes, char lastCommand, MouseState mouseState)
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
                        case Key.Up:
                        case Key.W:
                            intent.Intention = IntentEnum.MoveUp;
                            break;
                        case Key.S:
                        case Key.Down:
                            intent.Intention = IntentEnum.MoveDown;
                            break;
                        case Key.A:
                        case Key.Left:
                            intent.Intention = IntentEnum.MoveLeft;
                            break;
                        case Key.D:
                            intent.Intention = IntentEnum.MoveRight;
                            break;
                        //case Key.Right:
                        //    intent.Intention = IntentEnum.MoveRight;
                        //    break;
                        //case Key.NumPad7:
                        //    intent.Intention = IntentEnum.MoveTopLeft;
                        //    break;
                        //case Key.NumPad9:
                        //    intent.Intention = IntentEnum.MoveTopRight;
                        //    break;
                        //case Key.NumPad1:
                        //    intent.Intention = IntentEnum.MoveBottomLeft;
                        //    break;
                        //case Key.NumPad3:
                        //    intent.Intention = IntentEnum.MoveBottomRight;
                        //    break;
                        //case Key.NumPad5:
                        //    intent.Intention = IntentEnum.SkipTurn;

                        //    break;
                        case Key.Enter:
                            intent.Intention = IntentEnum.Enter;
                            break;
                        case Key.F:
                            intent.Intention = IntentEnum.LookAtMode;
                            break;
                        case Key.P:
                            intent.Intention = IntentEnum.PickUpItem;
                            break;
                        case Key.F5:
                            intent.Intention = IntentEnum.Quicksave;
                            break;
                        case Key.F9:
                            intent.Intention = IntentEnum.Quickload;
                            break;
                        case Key.Z:
                            if (lastCommand == 'z')
                            {
                                intent.Intention = IntentEnum.ZoomOut;
                            }
                            else if (lastCommand == 'Z')
                            {
                                intent.Intention = IntentEnum.ZoomIn;
                            }
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
