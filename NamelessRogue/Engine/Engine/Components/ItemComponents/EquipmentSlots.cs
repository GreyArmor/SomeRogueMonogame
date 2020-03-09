using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Components.ItemComponents
{


    public class EquipmentSlots : Component
    {
        public enum Slot
        {
            Head,
            Torso,
            LefHand,
            RightHand,
            Legs,
            Feet,
            Ring1,
            Ring2,
            Neck,
            Hands,
        }

        

        public List<Tuple<Slot, EquipmentSlot>> Slots { get; }

        public ItemsHolder Holder { get; }
        public NamelessGame Game { get; }

        public EquipmentSlots(ItemsHolder holder, NamelessGame game)
        {
            Slots = new List<Tuple<Slot, EquipmentSlot>>();
            Slots.Add(new Tuple<Slot, EquipmentSlot>(Slot.Head, new EquipmentSlot(Slot.Head)));
            Slots.Add(new Tuple<Slot, EquipmentSlot>(Slot.Torso, new EquipmentSlot(Slot.Torso)));

            var slotLeft = new EquipmentSlot(Slot.LefHand);
            var slotRight = new EquipmentSlot(Slot.RightHand);

            Slots.Add(new Tuple<Slot, EquipmentSlot>(Slot.LefHand, slotLeft));
            Slots.Add(new Tuple<Slot, EquipmentSlot>(Slot.RightHand, slotRight));

            Slots.Add(new Tuple<Slot, EquipmentSlot>(Slot.Feet, new EquipmentSlot(Slot.Feet)));
            Slots.Add(new Tuple<Slot, EquipmentSlot>(Slot.Legs, new EquipmentSlot(Slot.Legs)));
            Slots.Add(new Tuple<Slot, EquipmentSlot>(Slot.Neck, new EquipmentSlot(Slot.Neck)));
            Slots.Add(new Tuple<Slot, EquipmentSlot>(Slot.Ring1, new EquipmentSlot(Slot.Ring1)));
            Slots.Add(new Tuple<Slot, EquipmentSlot>(Slot.Ring2, new EquipmentSlot(Slot.Ring2)));
            Slots.Add(new Tuple<Slot, EquipmentSlot>(Slot.Hands, new EquipmentSlot(Slot.Hands)));

            Holder = holder;
            Game = game;
        }

        public void Equip(Equipment equipment, List<Slot> equipTo)
        {
            var slots = Slots.Where(x => equipTo.Contains(x.Item1));
            foreach (var tuple in slots)
            {
                var slot = tuple.Item2;
                if (slot.Equipment != null)
                {
                    TakeOff(slot.Equipment);
                }

                slot.Equipment = equipment;
            }

            Holder.GetItems().Remove(Holder.GetItems().FirstOrDefault(x=>x.GetId()==equipment.ParentEntityId));
        }

        public void TakeOff(Equipment equipment)
        {
            var slots = Slots.Where(x => x.Item2.Equipment == equipment);
            foreach (var tuple in slots)
            {
                tuple.Item2.Equipment = null;
                var itemEntity = Holder.GetItems().FirstOrDefault(x => x.GetId() == equipment.ParentEntityId);
                if (itemEntity == null)
                {
                    Holder.GetItems().Add(Game.GetEntity(equipment.ParentEntityId));
                }
            }
        }

        public override IComponent Clone()
        {
            throw new NotImplementedException();
        }
    }

    public class EquipmentSlot
    {
        public EquipmentSlot(EquipmentSlots.Slot slot)
        {
            Slot = slot;
        }

        public EquipmentSlots.Slot Slot { get; }
        public Equipment Equipment { get; set; }

    }
}
