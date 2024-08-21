using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Stats
{
    public class Stats : Component
    {
        public Stats()
        {}

        public SimpleStat Health { get; set; } = new SimpleStat(0, 0, 9999);
        public SimpleStat Speed { get; set; } = new SimpleStat(0, 0, 9999);
        public SimpleStat VisionRange { get; set; } = new SimpleStat(0, 0, 9999);
        public SimpleStat Weight { get; set; } = new SimpleStat(0, 0, 9999);
        public List<WeaponStat> WeaponStats { get; set; }
        public List<ResistanceStat> Resistances { get; set; }
        public List<ArmorStats> Armor { get; set;}

        public string FactionId { get; set; }

		public override IComponent Clone()
        {
            return new Stats()
            {
                Health = Health,
                Stamina = Stamina,
                VisionRange = VisionRange,
				FactionId = FactionId,
			};
        }
    }
}
