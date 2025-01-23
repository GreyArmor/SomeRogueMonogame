using NamelessRogue.Engine.Abstraction;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal interface IActivatedAbilityCommand
    {
        IEntity Ability { get; }
        IEntity Source { get; }
    }
}