using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace NamelessRogue.Engine.Engine.Input
{
    public interface IKeyIntentTraslator
    {
        List<Intent> Translate(Keys[] keyCodes, char lastCommand);
    }
}