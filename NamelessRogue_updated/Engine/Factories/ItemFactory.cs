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
    public  class ItemFactory {

        public static Entity CreateSword(int x, int y, int i, NamelessGame game)
        {
            Entity item = new Entity();
            item.AddComponent(new Item(ItemType.Weapon,2,ItemQuality.Normal,1,1,""));
            item.AddComponent(new Drawable('S', new Color(1f, 0, 0)));
            item.AddComponent(new Description("Sword " + i.ToString(), "A simple sword"));
            item.AddComponent(new Equipment(new List<Slot>(){Slot.LefHand,Slot.RightHand}));
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
            item.AddComponent(new Equipment(new List<Slot>() { Slot.Head}));
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
            item.AddComponent(new Equipment(new List<Slot>() { Slot.Feet}));
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
            item.AddComponent(new Equipment(new List<Slot>() { Slot.Legs }));
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
            item.AddComponent(new Equipment(new List<Slot>() { Slot.RightHand },
                                            new List<Slot>() { Slot.LefHand }));
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
            item.AddComponent(new Equipment(new List<Slot>() { Slot.Ring1 },
                                            new List<Slot>() { Slot.Ring2 }));
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
            item.AddComponent(new Equipment(new List<Slot>() { Slot.Neck}));
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
            item.AddComponent(new Equipment(new List<Slot>() { Slot.Torso}));
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

        public static Entity CreateLightAmmo(int x, int y, int i, int amount, NamelessGame game, AmmoLibrary ammoLibrary)
        {
            Entity item = new Entity();
            var revoAmmo = ammoLibrary.AmmoTypes.First();
            item.AddComponent(new Item(ItemType.Ammo, 0.01f, ItemQuality.Normal, amount, 1, ""));
            item.AddComponent(new Ammo(new AmmoType()));
            item.AddComponent(new Drawable(revoAmmo.Name[0], new Color(0f, 1f, 0)));
            item.AddComponent(new Description(revoAmmo.Name + i.ToString(), revoAmmo.Name));
            var position = new Position(x, y);
            item.AddComponent(position);
            game.WorldProvider.MoveEntity(item, position.p);
            return item;
        }

        public static Entity CreateRevolver(int x, int y, int i, NamelessGame game, AmmoLibrary ammoLibrary)
        {
            Entity item = new Entity();
            item.AddComponent(new Item(ItemType.Weapon, 2, ItemQuality.Normal, 1, 1, ""));
            item.AddComponent(new Drawable('R', new Color(1f, 0, 0)));
            item.AddComponent(new Description("Revolver " + i.ToString(), "A simple revolver"));
            item.AddComponent(new Equipment(new List<Slot>() { Slot.RightHand },
                                            new List<Slot>() { Slot.LefHand }));
            item.AddComponent(new WeaponStats(1,10,25,AttackType.Ranged, ammoLibrary.AmmoTypes.First(), 6,0));
            var position = new Position(x, y);
            item.AddComponent(position);
            game.WorldProvider.MoveEntity(item, position.p);
            return item;
        }

            public static Entity CreatePowerArmor(int x, int y, int i, NamelessGame game)
            {
                Entity item = new Entity();
                item.AddComponent(new Item(ItemType.Armor, 2, ItemQuality.Normal, 1, 1, ""));
                item.AddComponent(new Drawable('P', new Color(1f, 1f, 0)));
                item.AddComponent(new Description("Power armor " + i.ToString(), "A Power armor "));
                item.AddComponent(new Equipment(new List<Slot>()
                {
                    Slot.Head,
                    Slot.Feet,
                    Slot.Neck,
                    Slot.Legs,
                    Slot.Torso,
                    Slot.Hands
                }));
                var position = new Position(x, y);
                item.AddComponent(position);
                game.WorldProvider.MoveEntity(item, position.p);
                return item;
            }

    }
}
