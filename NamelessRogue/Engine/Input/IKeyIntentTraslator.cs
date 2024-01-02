using System.Collections.Generic;
using System.Windows.Forms;


namespace NamelessRogue.Engine.Input
{
    public interface IKeyIntentTraslator
    {
        List<Intent> Translate(Keys[] keyCodes, char lastCommand, MouseState mouseState);
    }
}