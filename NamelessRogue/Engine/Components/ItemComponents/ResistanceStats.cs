using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Stats
{
    public class ResistanceStat : Component
    {
        public ResistanceStat() { }

        public DamageType DamageType { get; set; }
        public SimpleStat Value { get; set; } = new SimpleStat(0, 0, 9999);

        public void ResetValue()
        {
            Value.Value = 0;
        }

        public void Add(ResistanceStat other)
        {
            Value.Value += other.Value.Value;
        }
    }
}
