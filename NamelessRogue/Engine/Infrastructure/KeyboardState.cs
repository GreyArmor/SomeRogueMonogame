﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Veldrid;

namespace NamelessRogue.Engine.Infrastructure
{
    public struct KeyboardState
    {
        public List<Key> Keys = new List<Key>();
        public KeyboardState(){}

        public KeyboardState(InputSnapshot snapshot) {
            Keys = snapshot.KeyEvents.Where(ev => ev.Down).Select(x => x.Key).ToList();
        }
    }
}
