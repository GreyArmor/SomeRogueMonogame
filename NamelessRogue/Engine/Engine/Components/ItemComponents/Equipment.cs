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
        public EquipmentSlots.Slot Slot { get; }
        public Equipment(IEntity parent, EquipmentSlots.Slot slot)
        {
            Parent = parent;
            Slot = slot;
        }

    }
}
