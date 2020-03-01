using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Engine.Components.ItemComponents
{
    public class ArmorStats : Component
    {
        public ArmorStats(int defenceValue, int evasionValue)
        {
            this.DefenceValue = defenceValue;
            this.EvasionValue = evasionValue;
        }

        public int DefenceValue { get; set; }
        public int EvasionValue { get; set; }
    }
}
