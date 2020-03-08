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

    public class WeaponStats : Component
    {
        public WeaponStats(int minimumDamage, int maximumDamage, int range, AttackType attackType, AmmoType ammoType,
            int ammoInClip, int currentAmmo)
        {
            this.MinimumDamage = minimumDamage;
            this.MaximumDamage = maximumDamage;
            this.Range = range;
            this.AttackType = attackType;
            this.AmmoType = ammoType;
            this.AmmoInClip = ammoInClip;
            this.CurrentAmmo = currentAmmo;
        }

        public int MinimumDamage { get; private set; }
        public int MaximumDamage { get; private set; }
        public int Range { get; private set; }
        public AttackType AttackType { get; private set; }
        public AmmoType AmmoType { get; private set; }
        public int AmmoInClip { get; private set; }
        public int CurrentAmmo { get; set; }


        public override IComponent Clone()
        {
            return new WeaponStats(MinimumDamage, MaximumDamage, Range, AttackType, AmmoType, AmmoInClip, CurrentAmmo);
        }
    }
}
