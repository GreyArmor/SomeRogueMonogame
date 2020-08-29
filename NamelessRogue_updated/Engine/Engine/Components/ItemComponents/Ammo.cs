using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Engine.Components.ItemComponents
{
    public class Ammo : Component
    {
        public AmmoType Type { get; set; }

        public Ammo(AmmoType type)
        {
            Type = type;
        }

        public override IComponent Clone()
        {
            return new Ammo(this.Type);
        }
    }
}
