using NamelessRogue.Engine.Engine.Components;
using NamelessRogue.Engine.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.Engine.Engine.Components.Stats;
using NamelessRogue.Engine.Engine.Components.UI;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.Engine.Engine.Utility;

namespace NamelessRogue.Engine.Engine.Factories
{
    public  class ItemFactory {

        public static Entity CreateSword(int x, int y, int i)
        {
            Entity item = new Entity();
            item.AddComponent(new Item(ItemType.Weapon,2));
            item.AddComponent(new Drawable('S', new Color(1f, 0, 0)));
            item.AddComponent(new Description("Sword " + i.ToString(), "A simple sword"));
            item.AddComponent(new Equipment(item, EquipmentSlots.Slot.RightArm));
            item.AddComponent(new Stats()
            {
                Attack = new SimpleStat(10,0,100),
                AttackSpeed = new SimpleStat(100,0,100)
            });
            //item.AddComponent(new JustCreated());
            //for debug;
            item.AddComponent(new Position(x, y));
            return item;
        }

    }
}
