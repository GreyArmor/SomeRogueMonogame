using NamelessRogue.Engine.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Environment
{
    public class AnimatedSpriteObject : Component
    {
        public AnimatedSpriteObject(string spritePath = "", bool infinteAnimation = false, AnimationType idleAnimationType = AnimationType.Idle)
        {
            SpritePath = spritePath;
            InfinteAnimation = infinteAnimation;
            IdleAnimationType = idleAnimationType;
            Sprite = SpriteLibrary.CreateSprite(spritePath);
        }

        public string SpritePath { get; }
        public bool InfinteAnimation { get; set; }
        public string CurrentAnimation { get; set; }

        public int CurrentAnimationTimeLeft { get; set; }

     //   public int CurrentAnimationLoops{ get; set; }
        public AnimationType IdleAnimationType { get; internal set; }

        public AnimatedSpriteNR Sprite { get; set; }


        public override IComponent Clone()
        {
            return new AnimatedSpriteObject(SpritePath, InfinteAnimation, IdleAnimationType );
        }
    }
}
