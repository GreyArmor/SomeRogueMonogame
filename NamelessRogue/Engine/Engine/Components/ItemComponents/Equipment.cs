using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Abstraction;

namespace NamelessRogue.Engine.Engine.Components.ItemComponents
{


    public class Equipment : Component
    {
        public IEntity Parent { get; }
        public List<EquipmentSlots.Slot> PossibleSlots { get; }
        public Equipment(IEntity parent, params EquipmentSlots.Slot[] slots)
        {
            Parent = parent;
            PossibleSlots = slots.ToList();
        }

    }
}
