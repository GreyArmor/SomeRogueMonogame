 

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.Generation.World.BoardPieces;
using NamelessRogue.Engine.Engine.Generation.World.Meta;

namespace NamelessRogue.Engine.Engine.Generation.World
{
    public class Civilization {


        public Civilization(string name, Color civColor)
        {
            Name = name;
            CivColor = civColor;
            Settlements = new List<Settlement>();
            Units = new List<MapUnit>();
            Buildings = new List<MapBuilding>();

        }
        public string Name { get; set; }

        public ProductionValue GlobalTreasury
        {
            get
            {
                return new ProductionValue(
                    Settlements.Sum(x=>x.Info.ProductionModifier.Food),
                    Settlements.Sum(x => x.Info.ProductionModifier.Manufacturing),
                    Settlements.Sum(x => x.Info.ProductionModifier.Culture),
                    Settlements.Sum(x => x.Info.ProductionModifier.Science),
                    Settlements.Sum(x => x.Info.ProductionModifier.Mana),
                    Settlements.Sum(x => x.Info.ProductionModifier.Health)

                );
            }
        }
        public Color CivColor { get; }
        public List<Settlement> Settlements { get; }
        public List<MapUnit> Units { get; }
        public List<MapBuilding> Buildings { get; }

        public int TechLevel { get; }

        public List<Technology> ResearchedTechnoholy { get; }


    }
}
