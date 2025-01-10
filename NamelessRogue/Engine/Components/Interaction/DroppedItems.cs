using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
    internal class DroppedItemscomponent : Component
    {
        public DroppedItemscomponent(IEnumerable<string> droppedItemIds)
        {
            DroppedItemIds = droppedItemIds;
        }

        public IEnumerable<string> DroppedItemIds { get; }

        public virtual IComponent Clone()
        {
            return new DroppedItemscomponent(DroppedItemIds);
        }
    }
}
