using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.ItemComponents
{
    public class Consumable : Component
    {
        public int Heath { get; internal set; }
        public int Energy { get; internal set; }
        public int Damage { get; internal set; }
        public int Armor { get; internal set; }
        public int Resistance { get; internal set; }

        public override IComponent Clone()
        {
            return new Consumable();
        }
    }
}
