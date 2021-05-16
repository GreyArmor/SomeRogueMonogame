using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.ItemComponents
{
    public enum AttackType
    {
        Close,
        Ranged,
    }

    public class WeaponStats : Component
    {
		public WeaponStats()
		{
		}

		public WeaponStats(int minimumDamage, int maximumDamage, int range, AttackType attackType = AttackType.Close, AmmoType ammoType = null,
            int ammoInClip = 0, int currentAmmo = 0)
        {
            this.MinimumDamage = minimumDamage;
            this.MaximumDamage = maximumDamage;
            this.Range = range;
            this.AttackType = attackType;
            this.AmmoType = ammoType;
            this.AmmoInClip = ammoInClip;
            this.CurrentAmmo = currentAmmo;
        }

        public int MinimumDamage { get; set; }
        public int MaximumDamage { get; set; }
        public int Range { get; set; }
        public AttackType AttackType { get; set; }
        public AmmoType AmmoType { get; set; }
        public int AmmoInClip { get; set; }
        public int CurrentAmmo { get; set; }


        public override IComponent Clone()
        {
            return new WeaponStats(MinimumDamage, MaximumDamage, Range, AttackType, AmmoType, AmmoInClip, CurrentAmmo);
        }
    }
}
