using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Generation;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Factories
{
    public class TimelineFactory
    {
        public static Entity CreateTimeline(NamelessGame game)
        {
            Entity timepline = new Entity();

            timepline.AddComponent(HistoryGenerator.BuildTimeline(game,new HistoryGenerator.HistoryGenerationSettings()));

            return timepline;
        }
    }
}
