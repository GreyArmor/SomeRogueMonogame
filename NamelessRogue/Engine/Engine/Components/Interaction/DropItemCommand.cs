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
    public class DropItemCommand : Component
    {
        public IEnumerable<IEntity> Items { get; }
        public ItemsHolder Holder { get; }
        public Point WhereToDrop { get; }

        public DropItemCommand(IEnumerable<IEntity> items, ItemsHolder holder, Point whereToDrop)
        {
            Items = items;
            Holder = holder;
            WhereToDrop = whereToDrop;
        }
    }
}
