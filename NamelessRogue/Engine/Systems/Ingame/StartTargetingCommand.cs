using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public enum TargetingMode
    {
        Enemies, Friends, None
    }
    public class StartTargetingCommand : ICommand
    {
        public TargetingMode TargetingMode { get; set; } = TargetingMode.None;
        public int Range { get; }

        public StartTargetingCommand(TargetingMode targetingMode, int range)
        {
            TargetingMode = targetingMode;
            Range = range;
        }
    }

   
}
