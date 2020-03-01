using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Engine.Components.ItemComponents
{
    public enum AttackType
    {
        Close,
        Ranged,
    }

    public enum AmmoType
    {
        None,
        Light,
        Medium,
        Heavy,
        Energy,
        Mana,
        Rocket,
        Explosive
    }

    public class WeaponStats : Component, ICloneable
    {
        public WeaponStats(int minimumDamage, int maximumDamage, int range, AttackType attackType, AmmoType ammoType,
            int ammoInClip)
        {
            this.MinimumDamage = minimumDamage;
            this.MaximumDamage = maximumDamage;
            this.Range = range;
            this.AttackType = attackType;
            this.AmmoType = ammoType;
            this.AmmoInClip = ammoInClip;
        }

        public int MinimumDamage { get; private set; }
        public int MaximumDamage { get; private set; }
        public int Range { get; private set; }
        public AttackType AttackType { get; private set; }
        public AmmoType AmmoType { get; private set; }
        public int AmmoInClip { get; private set; }

        public object Clone()
        {
            return new WeaponStats(MinimumDamage,MaximumDamage,Range,AttackType,AmmoType,AmmoInClip);
        }
    }
}
