using NamelessRogue.Engine.Components.ItemComponents;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Stats
{
    public class ArmorStats : Component
    {
        public ArmorStats() { }
        public DamageType DamageType { get; set; }
        public SimpleStat Value { get; set; } = new SimpleStat(0,0,9999);

        public override IComponent Clone()
        {
            return new ArmorStats();
        }

        public void ResetValue()
        {
            Value.Value = 0;
        }

        public void Add(ArmorStats other)
        {
            Value.Value += other.Value.Value;
        }

    }
}
