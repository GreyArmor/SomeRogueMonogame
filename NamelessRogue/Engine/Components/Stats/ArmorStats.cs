﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Stats
{
    public class ArmorStats
    {
        public ArmorStats() { }
        public DamageType DamageType { get; set; }
        public SimpleStat Value { get; set; } = new SimpleStat(0,0,9999);
    }
}
