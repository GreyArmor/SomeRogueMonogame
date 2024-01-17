using System.Collections.Generic;
using System.Windows.Forms;
using Veldrid;


namespace NamelessRogue.Engine.Input
{
    public interface IKeyIntentTraslator
    {
        List<Intent> Translate(Key[] keyCodes, char lastCommand, MouseState mouseState);
    }
}