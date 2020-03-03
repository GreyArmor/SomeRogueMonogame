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
            Neck,
            BothHands
        }

        

        public List<Tuple<Slot, EquipmentSlot>> Slots { get; }

        public ItemsHolder Holder { get; }

        public EquipmentSlots(ItemsHolder holder)
        {
            Slots = new List<Tuple<Slot, EquipmentSlot>>();
            Slots.Add(new Tuple<Slot, EquipmentSlot>(Slot.Head, new EquipmentSlot(Slot.Head)));
            Slots.Add(new Tuple<Slot, EquipmentSlot>(Slot.Torso, new EquipmentSlot(Slot.Torso)));

            var slotLeft = new EquipmentSlot(Slot.LeftArm);
            var slotRight = new EquipmentSlot(Slot.RightArm);

            Slots.Add(new Tuple<Slot, EquipmentSlot>(Slot.LeftArm, slotLeft));
            Slots.Add(new Tuple<Slot, EquipmentSlot>(Slot.RightArm, slotRight));

            Slots.Add(new Tuple<Slot, EquipmentSlot>(Slot.Feet, new EquipmentSlot(Slot.Feet)));
            Slots.Add(new Tuple<Slot, EquipmentSlot>(Slot.Legs, new EquipmentSlot(Slot.Legs)));
            Slots.Add(new Tuple<Slot, EquipmentSlot>(Slot.Neck, new EquipmentSlot(Slot.Neck)));
            Slots.Add(new Tuple<Slot, EquipmentSlot>(Slot.Ring1, new EquipmentSlot(Slot.Ring1)));
            Slots.Add(new Tuple<Slot, EquipmentSlot>(Slot.Ring2, new EquipmentSlot(Slot.Ring2)));

            Slots.Add(new Tuple<Slot, EquipmentSlot>(Slot.BothHands, slotLeft));
            Slots.Add(new Tuple<Slot, EquipmentSlot>(Slot.BothHands, slotRight));

            Holder = holder;
        }

        public void Equip(Equipment equipment, Slot equipTo)
        {
            var slots = Slots.Where(x => x.Item1 == equipTo);
            foreach (var tuple in slots)
            {
                var slot = tuple.Item2;
                if (slot != null)
                {
                    if (slot.Equipment != null)
                    {
                        TakeOff(equipTo);
                    }

                    slot.Equipment = equipment;
                    
                }
            }
            Holder.GetItems().Remove(equipment.Parent);
        }

        public void TakeOff(Slot slotName)
        {
            var slots = Slots.Where(x => x.Item1 == slotName || x.Item2.Slot==slotName);
            foreach (var tuple in slots)
            {
                var equipmentSlots = Slots.Select(es=>es).Where(x=>x.Item1==tuple.Item1);
                foreach (var equipmentSlot in equipmentSlots)
                {
                    var equip = equipmentSlot.Item2.Equipment;
                    equipmentSlot.Item2.Equipment = null;
                    if (equip != null)
                    {
                        if (!Holder.GetItems().Contains(equip.Parent))
                        {
                            Holder.GetItems().Add(equip.Parent);
                        }
                    }
                }
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
