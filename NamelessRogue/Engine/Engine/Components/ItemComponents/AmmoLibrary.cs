using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Engine.Components.ItemComponents
{
    public class AmmoLibrary : Component
    {
        public List<AmmoType> AmmoTypes { get; private set; } = new List<AmmoType>();


        public override IComponent Clone()
        {
            return new AmmoLibrary(){AmmoTypes = this.AmmoTypes.ToList()};
        }
    }
}
