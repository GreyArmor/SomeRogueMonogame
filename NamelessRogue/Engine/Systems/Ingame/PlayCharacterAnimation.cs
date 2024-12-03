using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class PlayCharacterAnimationCommand : ICommand
    {
        public PlayCharacterAnimationCommand(IEntity entity, string animationName) {
            Entity = entity;
            AnimationName = animationName;
        }

        public IEntity Entity { get; }
        public string AnimationName { get; }
    }
}
