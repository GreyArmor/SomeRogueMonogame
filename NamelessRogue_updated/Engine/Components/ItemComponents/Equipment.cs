using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Abstraction;

namespace NamelessRogue.Engine.Components.ItemComponents
{


    public class Equipment : Component
    {
        public List<List<Slot>> PossibleSlots { get; }
        public Equipment(params List<Slot>[] slots)
        {
            PossibleSlots = slots.ToList();
        }

        public override IComponent Clone()
        {
            return new Equipment(PossibleSlots.ToArray());
        }
    }
}
