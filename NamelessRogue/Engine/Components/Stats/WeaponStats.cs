using NamelessRogue.Engine.Components.ItemComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Stats
{
    public class WeaponStat
    {
        public WeaponStat() { }

        bool IsRanged { get; set; }

        bool UsesAmmo { get; set; }

        public AmmoType AmmoType {get; set;}
        public DamageType DamageType { get; set; }
         
        public SimpleStat Damage { get; set; } = new SimpleStat(0, 0, 9999);

        public SimpleStat ArmorPenetration { get; set; } = new SimpleStat(0, 0, 9999);

        public SimpleStat Range { get; set; } = new SimpleStat(0, 0, 9999);

        public SimpleStat MagazineSize { get; set; } = new SimpleStat(0, 0, 9999);


    }
}
