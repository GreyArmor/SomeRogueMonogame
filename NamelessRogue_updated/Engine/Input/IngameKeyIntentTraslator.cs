using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace NamelessRogue.Engine.Input
{
    public class IngameKeyIntentTraslator : IKeyIntentTraslator
    {
        public virtual List<Intent> Translate(Keys[] keyCodes, char lastCommand)
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
                        case Keys.NumPad5:
                            intent.Intention = IntentEnum.SkipTurn;
                            break;
                        case Keys.Enter:
                            intent.Intention = IntentEnum.Enter;
                            break;
                        case Keys.F:
                            intent.Intention = IntentEnum.LookAtMode;
                            break;
                        case Keys.P:
                            intent.Intention = IntentEnum.PickUpItem;
                            break;
                    }
                }
            }

            return result;
        }
    }
}
