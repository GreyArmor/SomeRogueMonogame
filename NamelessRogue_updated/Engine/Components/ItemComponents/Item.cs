using System;

namespace NamelessRogue.Engine.Components.ItemComponents
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

    public class Item : Component
    {
        private int _amount;

        public float Weight { get; set; }

        public ItemType Type { get; set; }

        public ItemQuality Quality { get; set; }

        public Item(ItemType type, float weight, ItemQuality quality, int amount, int level, string author)
        {
            Type = type;
            Weight = weight;
            Amount = amount;
            Quality = quality;
            Level = level;
            Author = author;
        }

        public int Amount
        {
            get { return _amount; }
            set
            {
                if (value < 1)
                {
                    throw new Exception("Amount could not be less than 1");
                }
                _amount = value;
            }
        }

        public int Level { get; set; }

        public string Author { get; set; }

        public override IComponent Clone()
        {
            return new Item(this.Type, this.Weight, this.Quality, this.Amount, this.Level, this.Author);
        }
    }
}
