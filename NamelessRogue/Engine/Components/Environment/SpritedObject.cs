using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Environment
{
    public class SpritedObject : Component
    {
        public SpritedObject(bool isStatic, string idleAnimation = "")
        {
            IsStatic = isStatic;
            IdleAnimation = idleAnimation;
        }

        public bool IsStatic { get; }
        public string IdleAnimation { get; set; }

        public string CurrentAnimation { get; set; }

        public float CurrentAnimationTimeLeft { get; set; }
        public override IComponent Clone()
        {
            return new SpritedObject(IsStatic, IdleAnimation);
        }
    }
}
