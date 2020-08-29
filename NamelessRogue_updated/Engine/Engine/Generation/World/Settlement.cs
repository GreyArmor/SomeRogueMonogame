using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.Generation.World.BoardPieces;
using NamelessRogue.Engine.Engine.Generation.World.Meta;

namespace NamelessRogue.Engine.Engine.Generation.World
{
    public class Settlement : MapBuilding
    {
        private Point WorldBoardPosition { get; set; }
        public Resorces Treasury { get; set; }
        public ConcreteSettlement Concrete { get; set; }
    }



}
