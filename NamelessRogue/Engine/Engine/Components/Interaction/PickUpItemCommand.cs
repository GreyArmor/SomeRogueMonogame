using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.ItemComponents;

namespace NamelessRogue.Engine.Engine.Components.Interaction
{
    public class PickUpItemCommand : Component
    {
        public IEnumerable<IEntity> Items { get; }
        public ItemsHolder Holder { get; }
        public Point WhereToPickUp { get; }

        public PickUpItemCommand(IEnumerable<IEntity> items, ItemsHolder holder, Point whereToPickUp)
        {
            Items = items;
            Holder = holder;
            WhereToPickUp = whereToPickUp;
        }

        public override IComponent Clone()
        {
           return new PickUpItemCommand(Items, Holder, WhereToPickUp);
        }
    }
}
