using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Generation.World;

namespace NamelessRogue.Engine.UI
{
    internal class StartNewGameCommand : ICommand
    {
        public StartNewGameCommand(TempCharacterArchtype tempCharacterArchtype, WorldTemplate template)
        {
            TempCharacterArchtype = tempCharacterArchtype;
            Template = template;
        }

        public TempCharacterArchtype TempCharacterArchtype { get; }
        public WorldTemplate Template { get; }
    }
}