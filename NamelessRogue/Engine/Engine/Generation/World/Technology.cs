using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Engine.Generation.World.BoardPieces;

namespace NamelessRogue.Engine.Engine.Generation.World
{
    public class Technology
    {
        public List<Technology> Prerequsites { get; set; } = new List<Technology>();
        public List<MapResource> OpensResources { get; set; } = new List<MapResource>();
        
        public int TechLevel { get; }

    }
}
