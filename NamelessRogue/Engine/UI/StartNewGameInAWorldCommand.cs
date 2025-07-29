using NamelessRogue.Engine.Abstraction;

namespace NamelessRogue.Engine.UI
{
    internal class StartNewGameInAWorldCommand : ICommand
    {
        public StartNewGameInAWorldCommand(string worldPath)
        {
            WorldPath = worldPath;
        }

        public string WorldPath { get; }
    }
}