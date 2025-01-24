using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Environment
{
    public class Fire : Component
    {
        public int StartTurn { get; set; }

        public int Duration { get; set; }

        public override IComponent Clone()
        {
            return new Fire();
        }
    }
}
