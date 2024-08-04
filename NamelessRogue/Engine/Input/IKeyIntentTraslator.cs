using NamelessRogue.Engine.Infrastructure;
using System.Collections.Generic;
using System.Windows.Forms;



namespace NamelessRogue.Engine.Input
{
    public interface IKeyIntentTraslator
    {
        List<Intent> Translate(Key[] keyCodes, char lastCommand, MouseState mouseState);
    }
}