using NamelessRogue.Engine.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace NamelessRogue.Engine.Input
{
    public class InventoryKeyIntentTranslator : IKeyIntentTraslator
    {
        public List<Intent> Translate(Key[] keyCodes, char lastCommand, MouseState mouseState)
		{
            List<Intent> result = new List<Intent>();
            ////TODO: Add dictionary for actions, based on game config files

            //if (keyCodes.Length == 0)
            //{
            //}
            //else
            //{
            //    for (int i = 0; i < keyCodes.Length; i++)
            //    {
            //        var keyCode = keyCodes[i];
            //        Intent intent = new Intent(keyCodes.ToList(),lastCommand);
            //        result.Add(intent);
            //        switch (keyCode)
            //        {
            //            case Key.Up:
            //            case Key.Number8:
            //                intent.Intention = IntentEnum.MoveUp;
            //                break;
            //            case Key.Number2:
            //            case Key.Down:
            //                intent.Intention = IntentEnum.MoveDown;
            //                break;
            //            case Key.Number4:
            //            case Key.Left:
            //                intent.Intention = IntentEnum.MoveLeft;
            //                break;
            //            case Key.Number6:
            //            case Key.Right:
            //                intent.Intention = IntentEnum.MoveRight;
            //                break;
            //            //case Key.NumPad7:
            //            //    intent.Intention = IntentEnum.MoveTopLeft;
            //            //    break;
            //            //case Key.NumPad9:
            //            //    intent.Intention = IntentEnum.MoveTopRight;
            //            //    break;
            //            //case Key.NumPad1:
            //            //    intent.Intention = IntentEnum.MoveBottomLeft;
            //            //    break;
            //            //case Key.NumPad3:
            //            //    intent.Intention = IntentEnum.MoveBottomRight;
            //            //    break;
            //            case Key.Enter:
            //                intent.Intention = IntentEnum.Enter;
            //                break;
            //            case Key.Escape:
            //                intent.Intention = IntentEnum.Escape;
            //                break;
            //            default:
            //                intent.Intention = IntentEnum.ConetextualHotkeyPressed;
            //                break;
                            
            //        }
            //    }
            //}

            return result;
        }

    }
}
