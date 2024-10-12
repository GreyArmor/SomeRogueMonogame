using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Inventory
{
    internal class EquipSystem : BaseSystem
    {
        public override HashSet<Type> Signature => new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            var playerHolder = namelessGame.PlayerEntity.GetComponentOfType<ItemsHolder>();
            var playerEquipment = namelessGame.PlayerEntity.GetComponentOfType<EquipmentSlots>();

            while (namelessGame.Commander.DequeueCommand<EquipOrTakeOffCommand>(out var equipOrTakeOffCommand))
            {
                var item = namelessGame.GetEntity(equipOrTakeOffCommand.ItemId);
                var equipmentComponent = item.GetComponentOfType<Equipment>();
                if (equipOrTakeOffCommand.Equip)
                {                   
                    playerEquipment.Equip(equipmentComponent, equipmentComponent, new List<Slot>() { equipOrTakeOffCommand.EquipTo });
                }
                else
                {
                    playerEquipment.TakeOff(equipmentComponent);
                }

                namelessGame.Commander.EnqueueCommand(new UpdateInventoryScreenCommand());


            }
        }
    }
}
