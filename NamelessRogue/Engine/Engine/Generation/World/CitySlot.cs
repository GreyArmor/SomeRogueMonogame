﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;

namespace NamelessRogue.Engine.Engine.Generation.World
{
    public class CitySlot
    {
        public CitySlot(BoundingBox placement)
        {
            Placement = placement;
        }
        public BoundingBox Placement { get; private set; }
        public bool Occupied { get; set; }
        public IEntity OccupiedBy { get; set; }
    }
}
