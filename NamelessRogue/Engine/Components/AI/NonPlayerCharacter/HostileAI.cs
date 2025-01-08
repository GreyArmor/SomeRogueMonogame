using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.AI.NonPlayerCharacter
{


    public enum HostileTurretState
    {
        Idle, Attacking
    }

    public class HostileTurretAI : Component
    {
        public HostileTurretAI()
        {
        }
        public IEntity Target { get; set; }

        public HostileTurretState State { get; set; }

        public override IComponent Clone()
        {
            return new HostileTurretAI()
            {
                Target = this.Target,
            };
        }
    }
}
