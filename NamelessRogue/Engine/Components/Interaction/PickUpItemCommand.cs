using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Utility;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class PickUpItemCommand : ICommand
    {
        public IEnumerable<Entity> Items { get; }
        public ItemsHolder Holder { get; }
        public bool CallMultipleChoiceDialog { get; }

        public PickUpItemCommand(IEnumerable<Entity> items, ItemsHolder holder, bool callMultipleChoiceDialog = false)
        {
            Items = items;
            Holder = holder;
            CallMultipleChoiceDialog = callMultipleChoiceDialog;
        }
    }
}
