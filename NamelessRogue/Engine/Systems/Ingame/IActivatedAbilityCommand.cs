using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal interface IActivatedAbilityCommand
    {
        Entity Ability { get; }
        Entity Source { get; }
    }
}