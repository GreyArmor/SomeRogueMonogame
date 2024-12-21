using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class AbilityBinder : Component
    {
        public AbilityBinder()
        {}

        public Dictionary<int, IEntity> AbilityBindings { get; set; } = new Dictionary<int, IEntity>();
    }
}
