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
        public int Seed { get; }

        public TimeLine(int seed)
        {
            Seed = seed;
            WorldBoardAtEveryAge = new List<TimelineLayer>();
        }
        public List<TimelineLayer> WorldBoardAtEveryAge { get; }

        public TimelineLayer CurrentTimelineLayer { get; set; }

        public override IComponent Clone()
        {
            throw new NotImplementedException();
        }
    }
}
