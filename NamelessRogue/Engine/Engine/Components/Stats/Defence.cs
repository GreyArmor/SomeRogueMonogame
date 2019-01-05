using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Engine.Components.Stats
{
    public class Defence : SimpleStat
    {
        public Defence(int value, int minValue, int maxValue) : base(value, minValue, maxValue)
        {
        }
    }
}
