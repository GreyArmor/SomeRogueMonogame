using NamelessRogue.Engine.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Environment.Spawners
{
    public class EmptySpaceSpawner : Spawner
    {
        public override Entity Spawn()
        {
            return null;
        }
    }
}
