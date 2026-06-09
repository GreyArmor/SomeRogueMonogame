using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Utility;
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

    public class OscillatorMovementAI : Component
    {
        public OscillatorMovementAI(Vector3Int from, Vector3Int to)
        {
            From = from;
            To = to;
        }

        public Vector3Int From { get; private set; }
        public Vector3Int To { get; }

        public bool MovesToTarget { get; set; } = true;

        public override IComponent Clone()
        {
            return new OscillatorMovementAI(From, To)
            {
            };
        }
    }
}
