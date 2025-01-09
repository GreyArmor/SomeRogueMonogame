using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components._3D;
using NamelessRogue.Engine.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimationType = NamelessRogue.Engine.Infrastructure.AnimationType;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class PlayCharacterAnimationCommand : ICommand
    {
        public PlayCharacterAnimationCommand(IEntity entity, AnimationType type, int durationMilisecods) {
            Entity = entity;
            Type = type;
            DurationMilisecods = durationMilisecods;
        }

        public IEntity Entity { get; }
        public AnimationType Type { get; }
        public int DurationMilisecods { get; }
    }
}
