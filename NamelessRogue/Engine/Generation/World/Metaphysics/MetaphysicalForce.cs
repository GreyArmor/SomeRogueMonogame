using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

using NamelessRogue.Engine.Generation.World.Meta;

namespace NamelessRogue.Engine.Generation.World
{
    public class MetaphysicalForce
    {
		public MetaphysicalForce()
		{
		}

		public MetaphysicalForce(string name, Color color)
        {
            Name = name;
            Color = color;
        }

        public string Name { get; set; }
        public Color Color { get; set; }
    }
}
