using NamelessRogue.Engine.Engine.Components;
using NamelessRogue.Engine.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.Engine.Engine.Components.Stats;
using NamelessRogue.Engine.Engine.Components.UI;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.Engine.Engine.Utility;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Factories
{
    public  class ItemFactory {

        public static Entity CreateSword(int x, int y, int i, NamelessGame game)
        {
            Entity item = new Entity();
            item.AddComponent(new Item(ItemType.Weapon,2,ItemQuality.Normal,1,1,""));
            item.AddComponent(new Drawable('S', new Color(1f, 0, 0)));
            item.AddComponent(new Description("Sword " + i.ToString(), "A simple sword"));
            item.AddComponent(new Equipment(EquipmentSlots.Slot.BothHands));
            item.AddComponent(new Stats()
            {
                Attack = new SimpleStat(10,0,100),
                AttackSpeed = new SimpleStat(100,0,100)
            });
            var position = new Position(x, y);
            item.AddComponent(position);
            game.WorldProvider.MoveEntity(item, position.p);
            return item;
        }

        public static Entity CreateHelmet(int x, int y, int i, NamelessGame game)
        {
            Entity item = new Entity();
            item.AddComponent(new Item(ItemType.Armor, 2, ItemQuality.Normal, 1, 1, ""));
            item.AddComponent(new Drawable('H', new Color(1f, 0, 0)));
            item.AddComponent(new Description("Helmet " + i.ToString(), "A simple helmet"));
            item.AddComponent(new Equipment(EquipmentSlots.Slot.Head));
            item.AddComponent(new Stats()
            {
                Attack = new SimpleStat(10, 0, 100),
                AttackSpeed = new SimpleStat(100, 0, 100)
            });
            var position = new Position(x, y);
            item.AddComponent(position);
            game.WorldProvider.MoveEntity(item, position.p);
            return item;
        }

        public static Entity CreateBoots(int x, int y, int i, NamelessGame game)
        {
            Entity item = new Entity();
            item.AddComponent(new Item(ItemType.Armor, 2, ItemQuality.Normal, 1, 1, ""));
            item.AddComponent(new Drawable('B', new Color(1f, 0, 0)));
            item.AddComponent(new Description("Boots " + i.ToString(), "Simple Boots"));
            item.AddComponent(new Equipment(EquipmentSlots.Slot.Feet));
            item.AddComponent(new Stats()
            {
                Attack = new SimpleStat(10, 0, 100),
                AttackSpeed = new SimpleStat(100, 0, 100)
            });
            var position = new Position(x, y);
            item.AddComponent(position);
            game.WorldProvider.MoveEntity(item, position.p);
            return item;
        }

        public static Entity CreatePants(int x, int y, int i, NamelessGame game)
        {
            Entity item = new Entity();
            item.AddComponent(new Item(ItemType.Armor, 2, ItemQuality.Normal, 1, 1, ""));
            item.AddComponent(new Drawable('P', new Color(1f, 0, 0)));
            item.AddComponent(new Description("Pants " + i.ToString(), "Simple pants"));
            item.AddComponent(new Equipment(EquipmentSlots.Slot.Legs));
            item.AddComponent(new Stats()
            {
                Attack = new SimpleStat(10, 0, 100),
                AttackSpeed = new SimpleStat(100, 0, 100)
            });
            var position = new Position(x, y);
            item.AddComponent(position);
            game.WorldProvider.MoveEntity(item, position.p);
            return item;
        }

        public static Entity CreateShield(int x, int y, int i, NamelessGame game)
        {
            Entity item = new Entity();
            item.AddComponent(new Item(ItemType.Armor, 2, ItemQuality.Normal, 1, 1, ""));
            item.AddComponent(new Drawable('S', new Color(1f, 1f, 0)));
            item.AddComponent(new Description("Shield " + i.ToString(), "Simple shield"));
            item.AddComponent(new Equipment(EquipmentSlots.Slot.LeftArm));
            item.AddComponent(new Stats()
            {
                Attack = new SimpleStat(10, 0, 100),
                AttackSpeed = new SimpleStat(100, 0, 100)
            });
            var position = new Position(x, y);
            item.AddComponent(position);
            game.WorldProvider.MoveEntity(item, position.p);
            return item;
        }

        public static Entity CreateRing(int x, int y, int i, NamelessGame game)
        {
            Entity item = new Entity();
            item.AddComponent(new Item(ItemType.Armor, 2, ItemQuality.Normal, 1, 1, ""));
            item.AddComponent(new Drawable('R', new Color(1f, 1f, 0)));
            item.AddComponent(new Description("Ring " + i.ToString(), "Simple ring"));
            item.AddComponent(new Equipment(EquipmentSlots.Slot.Ring1, EquipmentSlots.Slot.Ring2));
            item.AddComponent(new Stats()
            {
                Attack = new SimpleStat(10, 0, 100),
                AttackSpeed = new SimpleStat(100, 0, 100)
            });
            var position = new Position(x, y);
            item.AddComponent(position);
            game.WorldProvider.MoveEntity(item, position.p);
            return item;
        }

        public static Entity CreateCape(int x, int y, int i, NamelessGame game)
        {
            Entity item = new Entity();
            item.AddComponent(new Item(ItemType.Armor, 2, ItemQuality.Normal, 1, 1, ""));
            item.AddComponent(new Drawable('C', new Color(1f, 1f, 0)));
            item.AddComponent(new Description("Cape " + i.ToString(), "Simple cape"));
            item.AddComponent(new Equipment(EquipmentSlots.Slot.Neck));
            item.AddComponent(new Stats()
            {
                Attack = new SimpleStat(10, 0, 100),
                AttackSpeed = new SimpleStat(100, 0, 100)
            });
            var position = new Position(x, y);
            item.AddComponent(position);
            game.WorldProvider.MoveEntity(item, position.p);
            return item;
        }

        public static Entity CreatePlateMail(int x, int y, int i, NamelessGame game)
        {
            Entity item = new Entity();
            item.AddComponent(new Item(ItemType.Armor, 2, ItemQuality.Normal, 1, 1, ""));
            item.AddComponent(new Drawable('M', new Color(1f, 1f, 0)));
            item.AddComponent(new Description("Plate mail " + i.ToString(), "Simple plate mail"));
            item.AddComponent(new Equipment(EquipmentSlots.Slot.Torso));
            item.AddComponent(new Stats()
            {
                Attack = new SimpleStat(10, 0, 100),
                AttackSpeed = new SimpleStat(100, 0, 100)
            });
            var position = new Position(x, y);
            item.AddComponent(position);
            game.WorldProvider.MoveEntity(item, position.p);
            return item;
        }

        public static Entity CreateLightAmmo(int x, int y, int i, int amount, NamelessGame game)
        {
            Entity item = new Entity();
            item.AddComponent(new Item(ItemType.Ammo, 0.01f, ItemQuality.Normal, amount, 1, ""));
            item.AddComponent(new Ammo(AmmoType.Light));
            item.AddComponent(new Drawable('L', new Color(0f, 1f, 0)));
            item.AddComponent(new Description("Light ammo " + i.ToString(), "Light ammo"));
            var position = new Position(x, y);
            item.AddComponent(position);
            game.WorldProvider.MoveEntity(item, position.p);
            return item;
        }

    }
}
