using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Generation.World.Metaphysics
{
    public class AffinityCollection
    {
        public AffinityCollection(params MetaAffinity[] affinities)
        {
            MetaAffinities = new List<MetaAffinity>(affinities);
        }
        public List<MetaAffinity> MetaAffinities { get; }
    }
}
