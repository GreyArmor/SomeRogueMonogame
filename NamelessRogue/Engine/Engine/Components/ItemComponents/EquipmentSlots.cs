using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Engine.Components.ItemComponents
{


    public class EquipmentSlots : Component
    {
        public enum Slot
        {
            Head,
            Torso,
            LeftArm,
            RightArm,
            Legs,
            Feet,
            Ring1,
            Ring2,
            Ring3,
            Ring4,
            Neck
        }

        public Dictionary<Slot, EquipmentSlot> Slots { get; }
        public ItemsHolder Holder { get; }

        public EquipmentSlots(ItemsHolder holder)
        {
            Slots = new Dictionary<Slot, EquipmentSlot>();
            Slots.Add(Slot.Head, new EquipmentSlot(Slot.Head));
            Slots.Add(Slot.Torso, new EquipmentSlot(Slot.Torso));

            Slots.Add(Slot.LeftArm, new EquipmentSlot(Slot.LeftArm));
            Slots.Add(Slot.RightArm, new EquipmentSlot(Slot.RightArm));

            Slots.Add(Slot.Feet, new EquipmentSlot(Slot.Feet));
            Slots.Add(Slot.Legs, new EquipmentSlot(Slot.Legs));
          
            Slots.Add(Slot.Neck, new EquipmentSlot(Slot.Neck));
            Slots.Add(Slot.Ring1, new EquipmentSlot(Slot.Ring1));
            Slots.Add(Slot.Ring2, new EquipmentSlot(Slot.Ring2));
            Slots.Add(Slot.Ring3, new EquipmentSlot(Slot.Ring3));
            Slots.Add(Slot.Ring4, new EquipmentSlot(Slot.Ring4));
            Holder = holder;
        }

        public void Equip(Equipment equipment)
        {
            Slots.TryGetValue(equipment.Slot,out var slot);
            if (slot!=null)
            {
                if (slot.Equipment != null)
                {
                    TakeOff(equipment.Slot);
                }

                slot.Equipment = equipment;
                Holder.GetItems().Remove(equipment.Parent);
            }
        }

        public void TakeOff(Slot slotName)
        {
            Slots.TryGetValue(slotName, out var slot);
            if (slot != null)
            {
                var equip = slot.Equipment;
                slot.Equipment = null;
                Holder.GetItems().Add(equip.Parent);
            }
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
