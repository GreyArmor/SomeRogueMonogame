using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class CreateProjectileCommand : ICommand
    {
        public CreateProjectileCommand(Vector3Int from, Vector3Int to, DamageType damageType)
        {
            From = from;
            To = to;
            DamageType = damageType;
        }

        public Vector3Int From { get; }
        public Vector3Int To { get; }
        public DamageType DamageType { get; }
    }
}