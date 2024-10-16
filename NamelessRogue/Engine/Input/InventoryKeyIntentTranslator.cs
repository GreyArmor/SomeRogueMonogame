using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace NamelessRogue.Engine.Input
{
    public class InventoryKeyIntentTranslator : IKeyIntentTraslator
    {
        public List<Intent> Translate(Keys[] keyCodes, char lastCommand, MouseState mouseState)
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
                    Intent intent = new Intent(keyCodes.ToList(),lastCommand);
                    result.Add(intent);
                    switch (keyCode)
                    {
                        case Keys.Up:
                        case Keys.NumPad8:
                            intent.Intention = IntentEnum.MoveUp;
                            break;
                        case Keys.NumPad2:
                        case Keys.Down:
                            intent.Intention = IntentEnum.MoveDown;
                            break;
                        case Keys.NumPad4:
                        case Keys.Left:
                            intent.Intention = IntentEnum.MoveLeft;
                            break;
                        case Keys.NumPad6:
                        case Keys.Right:
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
                        case Keys.Enter:
                            intent.Intention = IntentEnum.Enter;
                            break;
                        case Keys.Escape:
                            intent.Intention = IntentEnum.Escape;
                            break;
                        case Keys.E:
                            intent.Intention = IntentEnum.Interact;
                            break;
                        case Keys.Add:
                            intent.Intention = IntentEnum.Add;
                            break;
                        case Keys.Subtract:
                            intent.Intention = IntentEnum.Substract;
                            break;
                        default:
                            break;

                    }
                }
            }

            return result;
        }

    }
}
