using NamelessRogue.Engine.Generation.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Generation.World.Tech
{
    public class TechnologyAge
    {

        public string Name { get; set; }

        public List<ItemBlueprint> OpenedItems { get; set; }

        private int RequiredResearchPoints { get; set; }

    }
}
