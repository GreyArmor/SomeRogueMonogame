using System.Collections.Generic;
using NamelessRogue.Engine.Abstraction;

namespace NamelessRogue.Engine.Engine.Components.ItemComponents
{
    public class ItemsHolder : Component {

        private List<IEntity> items;
        public ItemsHolder()
        {
            items = new List<IEntity>();
        }

        public void SetItems(List<IEntity> items) {
            this.items = items;
        }

        public List<IEntity> GetItems() {
            return items;
        }

        public override IComponent Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}
