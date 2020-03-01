using System;

namespace NamelessRogue.Engine.Engine.Components.ItemComponents
{
    public enum ItemType
    {
        Weapon,
        Armor,
        Consumable,
        Food,
        Ammo,
        Misc,
    }

    public enum ItemQuality
    {
        Terrible,
        Poor,
        Shoddy,
        Normal,
        Good,
        Excellent,
        Superb
    }

    public class Item : Component, ICloneable
    {

        public float Weight { get; set; }

        public ItemType Type { get; set; }

        public ItemQuality Quality { get; set; }

        public Item(ItemType type, float weight, ItemQuality quality, int amount, int level, string author)
        {
            Type = type;
            Weight = weight;
            Amount = amount;
        }

        public int Amount { get; set; }

        public int Level { get; set; }

        public string Author { get; set; }

        public object Clone()
        {
            return new Item(this.Type, this.Weight, this.Quality, this.Amount, this.Level, this.Author);
        }
    }
}
