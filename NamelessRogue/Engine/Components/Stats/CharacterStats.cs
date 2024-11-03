using NamelessRogue.Engine.Components.ItemComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Stats
{
    public class CharacterStats : Component
    {
        public CharacterStats()
        {}

        public SimpleStat Health { get; set; } = new SimpleStat(0, 0, 999);
        public SimpleStat Energy { get; set; } = new SimpleStat(500, 0, 999);
        public SimpleStat Speed { get; set; } = new SimpleStat(0, 0, 999);
        public SimpleStat VisionRange { get; set; } = new SimpleStat(0, 0, 999);
        public SimpleStat Weight { get; set; } = new SimpleStat(0, 0, 999);
        public List<WeaponStats> WeaponStats { get; set; } = new List<WeaponStats> ();
        public List<ResistanceStat> Resistances { get; set; } = new List<ResistanceStat>();
        public List<ArmorStats> Armor { get; set;} = new List<ArmorStats> { };

        public string FactionId { get; set; }

		public override IComponent Clone()
        {
            return new CharacterStats()
            {
                Health = Health,
                VisionRange = VisionRange,
				FactionId = FactionId,
			};
        }

        internal void Add(CharacterStats modifierStats)
        {
            Health.Value += modifierStats.Health.Value;
            Energy.Value += modifierStats.Energy.Value;
            Speed.Value += modifierStats.Speed.Value;
            VisionRange.Value += modifierStats.VisionRange.Value;
            Weight.Value += modifierStats.Weight.Value;
            WeaponStats.AddRange(modifierStats.WeaponStats);
            Resistances.AddRange(modifierStats.Resistances);
            Armor.AddRange(modifierStats.Armor);
        }

        internal void ResetValue()
        {
            Health.Value = 0;
            Energy.Value = 0;
            Speed.Value = 0;
            VisionRange.Value = 0;
            Weight.Value = 0;
            WeaponStats.Clear();
            Resistances.Clear();
            Armor.Clear();
        }

        public Dictionary<DamageType, int> GetArmorByTypes()
        {
            var result = new Dictionary<DamageType, int>();
            foreach (var armor in Armor)
            {
                if (result.ContainsKey(armor.DamageType))
                {
                    result[armor.DamageType] += armor.Value.Value;
                }
                else
                {
                    result.Add(armor.DamageType, armor.Value.Value);
                }
            }
            return result;
        }

        public Dictionary<DamageType, int> GetResistanceByTypes()
        {
            var result = new Dictionary<DamageType, int>();
            foreach (var resistance in Resistances)
            {
                if (result.ContainsKey(resistance.DamageType))
                {
                    result[resistance.DamageType] += resistance.Value.Value;
                }
                else
                {
                    result.Add(resistance.DamageType, resistance.Value.Value);
                }
            }
            return result;
        }

        public Dictionary<DamageType, Tuple<int, int>> GetWeaponsByTypes()
        {
            var result = new Dictionary<DamageType, Tuple<int,int>>();
            foreach (var weaponGroup in WeaponStats.GroupBy(x=>x.DamageType))
            {
                var weaponType = weaponGroup.Key;
                var minDamage = weaponGroup.Select(weapon=> weapon.MinimumDamage).Aggregate((a,b)=>a+b);
                var maxDamage = weaponGroup.Select(weapon => weapon.MaximumDamage).Aggregate((a, b) => a + b);
                result.Add(weaponGroup.Key, new Tuple<int, int>(minDamage, maxDamage));
            }
            return result;
        }

    }
}
