using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Infrastructure;
namespace NamelessRogue.Engine.Components.Interaction
{
    public class DropItemCommand : ICommand
    {
        public IEnumerable<Entity> Items { get; }
        public ItemsHolder Holder { get; }
        public Point WhereToDrop { get; }

        public DropItemCommand(IEnumerable<Entity> items, ItemsHolder holder, Point whereToDrop)
        {
            Items = items;
            Holder = holder;
            WhereToDrop = whereToDrop;
        }
    }
}
