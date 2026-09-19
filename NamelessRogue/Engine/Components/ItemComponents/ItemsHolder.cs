using System.Collections.Generic;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Components.ItemComponents
{
    public class ItemsHolder : Component {

        private List<Entity> items;
        public ItemsHolder()
        {
            items = new List<Entity>();
        }

		public List<Entity> Items { get { return items; } set { items = value; } }

		public override IComponent Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}
