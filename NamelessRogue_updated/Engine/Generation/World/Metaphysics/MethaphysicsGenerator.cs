using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Generation.World.Meta;

namespace NamelessRogue.Engine.Generation.World.Metaphysics
{
    public class MethaphysicsGenerator
    {
        //move to config files later
        public List<MetaphysicalForce> CreateDefautForces()
        {
            var result = new List<MetaphysicalForce>();

            var grey = new MetaphysicalForce("Greyness", Color.Gray);
            var array = new MetaphysicalForce("Arrangement", Color.DarkBlue);
            var disarray = new MetaphysicalForce("Disarray", Color.Purple);
            var beginning = new MetaphysicalForce("Beginning", Color.Black);
            var ending = new MetaphysicalForce("End", Color.Crimson);
            var mundane = new MetaphysicalForce("Mundane", Color.Brown);
            var divine = new MetaphysicalForce("Divine", Color.Teal);
            var voidForce = new MetaphysicalForce("Void", Color.White);

            result.Add(grey);
            result.Add(array);
            result.Add(disarray);
            result.Add(beginning);
            result.Add(ending);
            result.Add(mundane);
            result.Add(divine);
            result.Add(voidForce);

            return result;
        }
    }
}
