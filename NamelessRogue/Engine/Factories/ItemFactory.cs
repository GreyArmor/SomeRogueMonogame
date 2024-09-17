using System.Collections.Generic;
using System.Linq;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Factories
{
    public static class ItemFactory {

        public static Entity CreateGun(NamelessGame game)
        {
            Entity entity = new Entity();


            entity.AddComponent(new Drawable("gunIcon", new Color(1f)));
            entity.AddComponent(new Description("Test gun", "Created to test inventory system in 2024"));
            entity.AddComponent(new Item(ItemType.Weapon, 0, ItemQuality.Normal, 1, 1, "CorpoCorp Inc."));
            entity.AddComponent(new WeaponStats(1, 10, 10, AttackType.Ranged, AmmoType.LowCaliber, 20, 0));
          //  entity.AddComponent(new EquipmentSlot(Slot.RightHand));
            return entity;
        }
        public static Entity CreateRedGun(NamelessGame game)
        {
            Entity entity = new Entity();
            entity.AddComponent(new Drawable("gunRedIcon", new Color(1f)));
            entity.AddComponent(new Description("Red test gun", "The same as Test gun, but red"));
            entity.AddComponent(new Item(ItemType.Weapon, 0, ItemQuality.Normal, 1, 1, "CorpoCorp Inc."));
            entity.AddComponent(new WeaponStats(1, 10, 10, AttackType.Ranged, AmmoType.LowCaliber, 20, 0));
            //  entity.AddComponent(new EquipmentSlot(Slot.RightHand));
            return entity;

        }
    }
}
