using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Physical
{
    internal class ProjectileComponent : Component
    {
        public ProjectileComponent(Vector3Int from, Vector3Int to, float speed)
        {
            From = from;
            To = to;
            Speed = speed;
            var distance = (from - to).Length();
            FramesToReachDestination = (int)(distance / speed);
        }
        public Vector3Int From { get; }
        public Vector3Int To { get; }
        public float Speed { get; }
        public int FramesToReachDestination { get; set; }
        public int CurrentFrame { get; set; }
    }
}
