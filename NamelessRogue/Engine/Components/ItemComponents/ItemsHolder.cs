using System.Collections.Generic;
using NamelessRogue.Engine.Abstraction;

namespace NamelessRogue.Engine.Components.ItemComponents
{
    public class ItemsHolder : Component {

        private List<IEntity> items;
        public ItemsHolder()
        {
            items = new List<IEntity>();
        }

		public List<IEntity> Items { get { return items; } set { items = value; } }

		public override IComponent Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}
