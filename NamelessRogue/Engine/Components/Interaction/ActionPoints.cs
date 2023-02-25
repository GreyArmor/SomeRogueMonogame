using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class ActionPoints : Component
    {
        public int Points { get; set; }

        public override IComponent Clone()
        {
            return new ActionPoints()
            {
                Points = this.Points
            };
        }
    }
}
