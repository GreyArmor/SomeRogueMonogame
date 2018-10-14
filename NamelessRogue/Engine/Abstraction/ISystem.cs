using Microsoft.Xna.Framework;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Abstraction
{
    public interface ISystem
    {
        void Update(long gameTime, NamelessGame namelessGame);
    }

}

