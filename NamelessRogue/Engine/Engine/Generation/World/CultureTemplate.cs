using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Engine.Generation.World
{
    public class CultureTemplate
    {
        public CultureTemplate(NamesGenerator nameGen)
        {
            NameGen = nameGen;
        }

        private NamesGenerator NameGen { get; set; }
    }
}
