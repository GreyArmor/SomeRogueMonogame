using NamelessRogue.Engine.Generation.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NamelessRogue.Engine.Components.Interaction
{


    public class DroppedItem
    {
        public string ItemId { get; set; } 
        public int Probability { get; set; } = 0;
    }
    internal class DroppedItemsComponent : Component
    {
        public DroppedItemsComponent(IEnumerable<DroppedItem> droppedItems)
        {
            DroppedItems = droppedItems.ToList();
        }

        public IEnumerable<DroppedItem> DroppedItems { get; }

        public virtual IComponent Clone()
        {
            return new DroppedItemsComponent(DroppedItems);
        }
    }
}
