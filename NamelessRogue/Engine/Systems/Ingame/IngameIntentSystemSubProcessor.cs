using NamelessRogue.Engine.Input;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public interface IngameIntentSystemSubProcessor
    {
        public void Process(NamelessGame namelessGame, IngameIntentSystem system, Intent intent);
    }
}
