using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class TergeterComponent : Component
    {
        public int TabulationIndex { get; set; } = -1;
        public List<IEntity> Targets { get; set; } = new List<IEntity>();
    }
}
