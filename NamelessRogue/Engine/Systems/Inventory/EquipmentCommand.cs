using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.ItemComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ICommand = NamelessRogue.Engine.Abstraction.ICommand;

namespace NamelessRogue.Engine.Systems.Inventory
{
    internal class EquipOrTakeOffCommand : ICommand
    {
        public EquipOrTakeOffCommand(Guid itemId, bool isEquipping, Slot equipTo = Slot.Head)
        {
            ItemId = itemId;
            Equip = isEquipping;
            EquipTo = equipTo;
            TakeOff = !isEquipping;
        }

        public Guid ItemId { get; }
        public bool Equip { get; }
        public Slot EquipTo { get; }
        public bool TakeOff { get; }
    }
}
