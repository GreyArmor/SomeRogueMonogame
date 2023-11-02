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
        {
            Health = new SimpleStat(0, 0, 9999);
            Stamina = new SimpleStat(0, 0, 9999);
            Attack = new SimpleStat(0, 0, 9999);
            Defence = new SimpleStat(0, 0, 9999);
            AttackSpeed = new SimpleStat(0, 0, 9999);
            MoveSpeed = new SimpleStat(0, 0, 9999);
			AttackRange = new SimpleStat(0, 0, 9999);
            VisionRange = new SimpleStat(0, 0, 9999);
		}
        
        public SimpleStat Health { get; set; }
        public SimpleStat Stamina { get; set; }
        public SimpleStat Attack { get; set; }
        public SimpleStat Defence { get; set; }
        public SimpleStat AttackSpeed { get; set; }
        public SimpleStat MoveSpeed { get; set; }
        public SimpleStat AttackRange { get; set; }
		public SimpleStat VisionRange { get; set; }

        public string FactionId { get; set; }

		public override IComponent Clone()
        {
            return new Stats()
            {
                Attack = Attack,
                AttackSpeed = AttackSpeed,
                Defence = Defence,
                Health = Health,
                MoveSpeed = MoveSpeed,
                Stamina = Stamina,
                AttackRange = AttackRange,
                VisionRange = VisionRange,
				FactionId = FactionId,
			};
        }
    }
}
