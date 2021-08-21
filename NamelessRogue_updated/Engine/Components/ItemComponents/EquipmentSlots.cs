using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Components.ItemComponents
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


    public class EquipmentSlots : Component
    {       

        public List<Tuple<Slot, EquipmentSlot>> Slots { get; set; } = new List<Tuple<Slot, EquipmentSlot>>();

        public ItemsHolder Holder { get; }
        private NamelessGame Game;

        public EquipmentSlots(ItemsHolder holder, NamelessGame game)
        {
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

		public EquipmentSlots()
		{
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

            Holder.Items.Remove(Holder.Items.FirstOrDefault(x=>x.Id==equipment.ParentEntityId));
        }

        public void TakeOff(Equipment equipment)
        {
            var slots = Slots.Where(x => x.Item2.Equipment == equipment);
            foreach (var tuple in slots)
            {
                tuple.Item2.Equipment = null;
                var itemEntity = Holder.Items.FirstOrDefault(x => x.Id == equipment.ParentEntityId);
                if (itemEntity == null)
                {
                    Holder.Items.Add(Game.GetEntity(equipment.ParentEntityId));
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
		public EquipmentSlot()
		{
		}

		public EquipmentSlot(Slot slot)
        {
            Slot = slot;
        }

        public Slot Slot { get; set; }
        public Equipment Equipment { get; set; }

    }
}
