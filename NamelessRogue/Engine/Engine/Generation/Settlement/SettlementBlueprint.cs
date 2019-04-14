using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Infrastructure;

namespace NamelessRogue.Engine.Engine.Generation.Settlement
{
    public enum SettlmentSlot
    {
        Road,
        Building,
        Water,
    }
    public class SettlementBlueprint
    {
        private readonly Action<SettlementBlueprint> settlementGenerationStrategy;

        public SettlementBlueprint(Action<SettlementBlueprint> settlementGenerationStrategy)
        {
            this.settlementGenerationStrategy = settlementGenerationStrategy;
            SettlmentSlots = new SettlmentSlot[Constants.CitySquare / Constants.CitySlotDimensions][];
            for (int i = 0; i < SettlmentSlots.Length; i++)
            {
                SettlmentSlots[i] = new SettlmentSlot[Constants.CitySquare / Constants.CitySlotDimensions];
            }
        }
        public SettlmentSlot[][] SettlmentSlots { get; set; }

        public void GenerateOnLand(IChunkProvider worldProvider, Point center)
        {

        }
       



    }
}
