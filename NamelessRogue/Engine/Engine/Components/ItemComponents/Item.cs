using System;

namespace NamelessRogue.Engine.Engine.Components.ItemComponents
{
    public enum ItemType
    {
        Weapon,
        Armor,
        Consumable,
        Misc,
    }
    public class Item : Component {
       
        public float Weight { get; set; }

        public ItemType Type { get; set; }

        public Item(ItemType type, float weight)
        {
            Type = type;
            Weight = weight;
        }
    }
}
