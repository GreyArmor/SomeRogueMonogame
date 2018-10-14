using System.Collections.Generic;

namespace NamelessRogue.Engine.Engine.Components.ItemComponents
{
    public class ItemsHolder : Component {

        private List<Item> items;
        public ItemsHolder()
        {
            items = new List<Item>();
        }

        public void setItems(List<Item> items) {
            this.items = items;
        }

        public List<Item> getItems() {
            return items;
        }
    }
}
