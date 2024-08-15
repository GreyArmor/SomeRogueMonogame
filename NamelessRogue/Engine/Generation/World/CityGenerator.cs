using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Generation.Noise;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Generation.World
{
    internal class CityGenerator
    {
        public InternalRandom Random { get; set; }
        public List<Waypoints> BorderLines { get; set; } = new List<Waypoints>();

        public CityGenerator(InternalRandom random)
        {
            Init(random);
        }

        public void Init(InternalRandom random)
        {
            Random = random;
            //TerrainNoises.Add(noise2);
            //TerrainNoises.Add(noise3);
        }

        public CityGenerator()
        {
        }
    }
}
