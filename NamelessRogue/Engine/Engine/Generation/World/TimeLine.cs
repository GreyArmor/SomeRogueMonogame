using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Engine.Components;

namespace NamelessRogue.Engine.Engine.Generation.World
{
    public class TimeLine : Component
    {
        public TimeLine()
        {
            WorldBoardAtEveryAge = new List<WorldBoard>();
        }
        public List<WorldBoard> WorldBoardAtEveryAge { get; }

        public WorldBoard CurrentWorldBoard { get; set; }
    }
}
