
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Abstraction
{
    public interface IDrawSystem : ISystem
    {
        void Draw(NamelessGame game);
    }
}