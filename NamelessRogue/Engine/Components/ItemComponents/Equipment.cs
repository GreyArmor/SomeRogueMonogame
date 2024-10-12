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
        public List<Slot> PossibleSlots { get; }
        public Equipment(params Slot[] slots)
        {
            PossibleSlots = slots.ToList();
        }

		public Equipment()
		{
		}

		public override IComponent Clone()
        {
            return new Equipment(PossibleSlots.ToArray());
        }
    }
}
