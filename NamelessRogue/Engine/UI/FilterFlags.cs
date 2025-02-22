using NamelessRogue.Engine.Components.ItemComponents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NamelessRogue.Engine.UI
{
    public class FilterFlags
    {
        public bool[] FilterArray = new bool[7];
        public List<List<ItemType>> Filters = new List<List<ItemType>>();

        public FilterFlags()
        {
            Filters.AddRange(new List<ItemType>[7]);
            Filters[0] = Enum.GetValues(typeof(ItemType)).Cast<ItemType>().ToList();
            Filters[1] = new List<ItemType>() { ItemType.Weapon };
            Filters[2] = new List<ItemType>() { ItemType.Armor };
            Filters[3] = new List<ItemType>() { ItemType.Consumable };
            Filters[4] = new List<ItemType>() { ItemType.Supplies };
            Filters[5] = new List<ItemType>() { ItemType.Ammo };
            Filters[6] = new List<ItemType>() { ItemType.Misc };
        }
        public bool All { get { return FilterArray[0]; } set { FilterArray[0] = value; } }
        public bool Weapons { get { return FilterArray[1]; } set { FilterArray[1] = value; } }
        public bool Armor { get { return FilterArray[2]; } set { FilterArray[2] = value; } }
        public bool Consumables { get { return FilterArray[3]; } set { FilterArray[3] = value; } }
        public bool Food { get { return FilterArray[4]; } set { FilterArray[4] = value; } }
        public bool Ammo { get { return FilterArray[5]; } set { FilterArray[5] = value; } }
        public bool Misc { get { return FilterArray[6]; } set { FilterArray[6] = value; } }
    }
}
