using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Physical
{
    public class StairsComponent : Component
    {
        public StairsComponent()
        {
        }
        public override IComponent Clone()
        {
            return new StairsComponent();
        }
    }
}
