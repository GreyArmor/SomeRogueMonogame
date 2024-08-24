using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Environment.Spawners
{
    internal class WallSpawner : Spawner
    {
        public override Entity Spawn()
        {
            return TerrainFurnitureFactory.WallEntity;
        }
    }
}
