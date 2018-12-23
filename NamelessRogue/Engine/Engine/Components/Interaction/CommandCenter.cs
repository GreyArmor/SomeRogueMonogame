﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Engine.Components.Interaction
{
    public class CommandCenter : Component
    {
        public List<object> Commands { get; set; } = new List<object>();
    }
}