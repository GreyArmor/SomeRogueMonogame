using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace NamelessRogue.Engine.Input
{
    public interface IKeyIntentTraslator
    {
        List<Intent> Translate(Keys[] keyCodes, char lastCommand, MouseState mouseState);
    }
}