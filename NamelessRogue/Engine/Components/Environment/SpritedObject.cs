using NamelessRogue.Engine.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Environment
{
    public class SpritedObject : Component
    {
        public SpritedObject(bool isStatic, string spriteId = "", string idleAnimation = "", bool infinteAnimation = false, AnimationType idleAnimationType = AnimationType.Idle)
        {
            IsStatic = isStatic;
            SpriteId = spriteId;
            IdleAnimation = idleAnimation;
            InfinteAnimation = infinteAnimation;
            CurrentAnimation = idleAnimation;
            IdleAnimationType = idleAnimationType;
        }

        public bool IsStatic { get; }
        public string SpriteId { get; }
        public string IdleAnimation { get; set; }
        public bool InfinteAnimation { get; }
        public string CurrentAnimation { get; set; }

        public float CurrentAnimationTimeLeft { get; set; }
        public AnimationType IdleAnimationType { get; internal set; }

        public override IComponent Clone()
        {
            return new SpritedObject(IsStatic, SpriteId, IdleAnimation);
        }
    }
}
