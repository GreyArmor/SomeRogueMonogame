
using NamelessRogue.Engine.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace NamelessRogue.Engine.Input
{
    public class PickUpKeyIntentTranslator : IKeyIntentTraslator
    {
        public List<Intent> Translate(Key[] keyCodes, char lastCommand, MouseState mouseState)
		{
            List<Intent> result = new List<Intent>();
            //////TODO: Add dictionary for actions, based on game config files

            //if (keyCodes.Length == 0)
            //{
            //}
            //else
            //{
            //    for (int i = 0; i < keyCodes.Length; i++)
            //    {
            //        var keyCode = keyCodes[i];
            //        Intent intent = new Intent(keyCodes.ToList(), lastCommand);
            //        result.Add(intent);
            //        switch (keyCode)
            //        {
            //            case Key.Up:
            //                intent.Intention = IntentEnum.MoveUp;
            //                break;
            //            case Key.Down:
            //                intent.Intention = IntentEnum.MoveDown;
            //                break;
            //            case Key.Left:
            //                intent.Intention = IntentEnum.MoveLeft;
            //                break;
            //            case Key.Right:
            //                intent.Intention = IntentEnum.MoveRight;
            //                break;
            //            case Key.Enter:
            //                intent.Intention = IntentEnum.Enter;
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
