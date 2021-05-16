using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Generation.World.Metaphysics
{
    public class MetaAffinity
    {
        public MetaAffinity(MetaphysicalForce force, float value)
        {
            Force = force;
            Value = value;
        }

        public MetaphysicalForce Force { get; }
        public float Value { get; set; }

    }
}
