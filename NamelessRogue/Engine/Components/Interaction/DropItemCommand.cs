using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Abstraction;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class DropItemCommand : ICommand
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
