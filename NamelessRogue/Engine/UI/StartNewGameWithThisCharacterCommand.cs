using NamelessRogue.Engine.Abstraction;

namespace NamelessRogue.Engine.UI
{
    internal class StartNewGameWithThisCharacterCommand : ICommand
    {
        public StartNewGameWithThisCharacterCommand(TempCharacterArchtype tempCharacterArchtype)
        {
        }

        public TempCharacterArchtype TempCharacterArchtype { get; }
    }
}