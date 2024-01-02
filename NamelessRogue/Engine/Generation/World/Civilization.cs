 

using System.Collections.Generic;
using System.Linq;
using SharpDX;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Generation.World.BoardPieces;
using NamelessRogue.Engine.Generation.World.Diplomacy;
using NamelessRogue.Engine.Generation.World.Meta;
using NamelessRogue.Engine.Generation.World.Tech;
using NamelessRogue.Engine.Serialization;
using Color = NamelessRogue.Engine.Utility.Color;
namespace NamelessRogue.Engine.Generation.World
{
    public class Civilization : Component {
		public Civilization()
		{
		}

		public Civilization(string name, Color civColor, CultureTemplate cultureTemplate)
        {
            Name = name;
            CivColor = civColor;
            CultureTemplate = cultureTemplate;
            Settlements = new List<Settlement>();
            Armies = new List<Army>();
            Buildings = new List<MapBuilding>();

        }
        public string Name { get; set; }

        public Resorces GlobalTreasury
        {
            get
            {
                return new Resorces(
                    Settlements.Sum(x=>x.Treasury.Population),
                    Settlements.Sum(x => x.Treasury.Wealth),
                    Settlements.Sum(x => x.Treasury.Supply),
                    Settlements.Sum(x => x.Treasury.Mana),
                    Settlements.Sum(x => x.Treasury.Influence)
                );
            }
        }
        public Color CivColor { get; }
        public CultureTemplate CultureTemplate { get; }
        public List<Settlement> Settlements { get; }
        public List<Army> Armies { get; }
        public List<MapBuilding> Buildings { get; }

        public Dictionary<Civilization,PoliticalRelation> Relations { get; set; }

    }
}
